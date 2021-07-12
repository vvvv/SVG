using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Xml;
using Svg.Transforms;

namespace Svg
{
    // Define as true if SvgElementBenchmarks is run.
#if true
    public class EmptyClass
    {
    }

    public partial class SvgElementEmpty : SvgElement
    {
        public override SvgElement DeepCopy()
        {
            throw new NotImplementedException();
        }
    }
#endif
    /// <summary>
    /// The base class of which all SVG elements are derived from.
    /// </summary>
    public abstract partial class SvgElement : ISvgElement, ISvgTransformable, ICloneable, ISvgNode
    {
        internal const int StyleSpecificity_PresAttribute = 0;
        internal const int StyleSpecificity_InlineStyle = 1 << 16;
#if !USE_SOURCE_GENERATORS
        //optimization
        protected class PropertyAttributeTuple
        {
            public PropertyDescriptor Property;
            public SvgAttributeAttribute Attribute;
        }

        protected class EventAttributeTuple
        {
            public FieldInfo Event;
            public SvgAttributeAttribute Attribute;
        }

        //reflection cache
        private IEnumerable<PropertyAttributeTuple> _svgPropertyAttributes;
        private IEnumerable<EventAttributeTuple> _svgEventAttributes;
#endif
        internal SvgElement _parent;
        private string _elementName;
        private SvgAttributeCollection _attributes;
        private EventHandlerList _eventHandlers;
        private SvgElementCollection _children;
        private static readonly object _loadEventKey = new object();
        private Matrix _graphicsTransform;
        private Region _graphicsClip;
        private SvgCustomAttributeCollection _customAttributes;
        private List<ISvgNode> _nodes;
        private Dictionary<string, SortedDictionary<int, string>> _styles;

        /// <summary>
        /// Add style.
        /// </summary>
        /// <param name="name">The style name.</param>
        /// <param name="value">The style value.</param>
        /// <param name="specificity">The specificity value.</param>
        public void AddStyle(string name, string value, int specificity)
        {
            if (_styles == null || !_styles.TryGetValue(name, out var rules))
            {
                rules = new SortedDictionary<int, string>();
                if (_styles == null)
                {
                    _styles = new Dictionary<string, SortedDictionary<int, string>>();
                }
                _styles[name] = rules;
            }
            while (rules.ContainsKey(specificity)) ++specificity;
            rules[specificity] = value;
        }

        /// <summary>
        /// Flush styles.
        /// </summary>
        /// <param name="children">If true, flush styles to the children.</param>
        public void FlushStyles(bool children = false)
        {
            FlushStyles();
            if (children && HasChildren())
            {
                foreach (var child in Children)
                {
                    child.FlushStyles(children);
                }
            }
        }

        private void FlushStyles()
        {
            if (_styles != null && _styles.Any())
            {
                Dictionary<string, SortedDictionary<int, string>> styles = null;
                foreach (var s in _styles)
                {
                    if (!SvgElementFactory.SetPropertyValue(this, s.Key, s.Value.Last().Value, OwnerDocument, true))
                    {
                        if (styles == null)
                        {
                            styles = new Dictionary<string, SortedDictionary<int, string>>();
                        }
                        styles.Add(s.Key, s.Value);
                    }
                }
                _styles = styles;
            }
        }

        public bool ContainsAttribute(string name)
        {
            return ((this._attributes != null && this._attributes.ContainsKey(name))
                    || (this._customAttributes != null && this._customAttributes.ContainsKey(name))
                    || (_styles != null && _styles.TryGetValue(name, out var rules))
                    && (rules.ContainsKey(StyleSpecificity_InlineStyle) || rules.ContainsKey(StyleSpecificity_PresAttribute)));
        }
        public bool TryGetAttribute(string name, out string value)
        {
            object objValue;
            if (this._attributes != null && this._attributes.TryGetValue(name, out objValue))
            {
                value = objValue.ToString();
                return true;
            }
            if (this._customAttributes != null && this._customAttributes.TryGetValue(name, out value))
            {
                return true;
            }
            if (_styles != null && _styles.TryGetValue(name, out var rules))
            {
                // Get staged styles that are
                if (rules.TryGetValue(StyleSpecificity_InlineStyle, out value)) return true;
                if (rules.TryGetValue(StyleSpecificity_PresAttribute, out value)) return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Gets the elements namespace as a string.
        /// </summary>
        protected internal string ElementNamespace { get; protected set; } = SvgNamespace.UriString;

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        protected internal string ElementName
        {
            get
            {
                if (string.IsNullOrEmpty(this._elementName))
                {
#if USE_SOURCE_GENERATORS
                    // There is special case for SvgDocument as valid attribute is only set on SvgFragment.
                    if (SvgElements.ElementNames.TryGetValue(this.GetType(), out var elementName))
                    {
                        this._elementName = elementName;
                    }
                    else if (this is SvgDocument)
                    {
                        // The SvgDocument does not have SvgElement attribute set, instead the attitude is used on SvgFragment so there would be duplicate im dictionary.
                        // The SvgDocument is not valid Svg element (that is SvgFragment) and is mainly used as abstraction for document reading and writing.
                        // The ElementName for SvgDocument is set explicitly here as that is the exception to attribute convention used accross codebase.
                        this._elementName = "svg";
                    }
#else
                    var attr = TypeDescriptor.GetAttributes(this).OfType<SvgElementAttribute>().SingleOrDefault();
                    if (attr != null)
                    {
                        this._elementName = attr.ElementName;
                    }
#endif
                }

                return this._elementName;
            }
            internal set { this._elementName = value; }
        }

        /// <summary>
        /// Gets or sets the color <see cref="SvgPaintServer"/> of this element which drives the currentColor property.
        /// </summary>
        [SvgAttribute("color")]
        public virtual SvgPaintServer Color
        {
            get { return GetAttribute("color", true, SvgPaintServer.NotSet); }
            set { Attributes["color"] = value; }
        }

        /// <summary>
        /// Gets or sets the content of the element.
        /// </summary>
        private string _content;
        public virtual string Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (_content != null)
                {
                    var oldVal = _content;
                    _content = value;
                    if (_content != oldVal)
                        OnContentChanged(new ContentEventArgs { Content = value });
                }
                else
                {
                    _content = value;
                    OnContentChanged(new ContentEventArgs { Content = value });
                }
            }
        }

        /// <summary>
        /// Gets an <see cref="EventHandlerList"/> of all events belonging to the element.
        /// </summary>
        protected virtual EventHandlerList Events
        {
            get
            {
                if (this._eventHandlers == null)
                {
                    this._eventHandlers = new EventHandlerList();
                }
                return this._eventHandlers;
            }
        }

        /// <summary>
        /// Occurs when the element is loaded.
        /// </summary>
        public event EventHandler Load
        {
            add { this.Events.AddHandler(_loadEventKey, value); }
            remove { this.Events.RemoveHandler(_loadEventKey, value); }
        }

        /// <summary>
        /// Gets a collection of all child <see cref="SvgElement"/> objects.
        /// </summary>
        public virtual SvgElementCollection Children
        {
            get
            {
                if (this._children == null)
                {
                    this._children = new SvgElementCollection(this);
                }
                return this._children;
            }
        }

        public IList<ISvgNode> Nodes
        {
            get
            {
                if (this._nodes == null)
                {
                    this._nodes = new List<ISvgNode>();
                }
                return this._nodes;
            }
        }

        public IEnumerable<SvgElement> Descendants()
        {
            return this.AsEnumerable().Descendants();
        }
        private IEnumerable<SvgElement> AsEnumerable()
        {
            yield return this;
        }

        /// <summary>
        /// Gets a value to determine whether the element has children.
        /// </summary>
        public virtual bool HasChildren()
        {
            return (this._children != null && this._children.Count > 0);
        }

        /// <summary>
        /// Gets a value to determine whether the element has nodes.
        /// </summary>
        public bool HasNodes()
        {
            return (this._nodes != null && this._nodes.Count > 0);
        }

        /// <summary>
        /// Gets a value to determine whether the element has attributes.
        /// </summary>
        public bool HasAttributes()
        {
            return (this._attributes != null && this._attributes.Count > 0);
        }

        /// <summary>
        /// Gets a value to determine whether the element has custom attributes.
        /// </summary>
        public bool HasCustomAttributes()
        {
            return (this._customAttributes != null && this._customAttributes.Count > 0);
        }

        /// <summary>
        /// Gets the parent <see cref="SvgElement"/>.
        /// </summary>
        /// <value>An <see cref="SvgElement"/> if one exists; otherwise null.</value>
        public virtual SvgElement Parent
        {
            get { return this._parent; }
        }

        public IEnumerable<SvgElement> Parents
        {
            get
            {
                var curr = this;
                while (curr.Parent != null)
                {
                    curr = curr.Parent;
                    yield return curr;
                }
            }
        }
        public IEnumerable<SvgElement> ParentsAndSelf
        {
            get
            {
                var curr = this;
                yield return curr;
                while (curr.Parent != null)
                {
                    curr = curr.Parent;
                    yield return curr;
                }
            }
        }

        /// <summary>
        /// Gets the owner <see cref="SvgDocument"/>.
        /// </summary>
        public virtual SvgDocument OwnerDocument
        {
            get
            {
                if (this is SvgDocument)
                {
                    return this as SvgDocument;
                }
                else
                {
                    if (this.Parent != null)
                        return Parent.OwnerDocument;
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// Gets a collection of element attributes.
        /// </summary>
        protected internal virtual SvgAttributeCollection Attributes
        {
            get
            {
                if (this._attributes == null)
                {
                    this._attributes = new SvgAttributeCollection(this);
                    //subscribe to attribute events
                    this._attributes.AttributeChanged += Attributes_AttributeChanged;
                }

                return this._attributes;
            }
        }

        protected bool Writing { get; set; }

        protected internal TAttributeType GetAttribute<TAttributeType>(string attributeName, bool inherited, TAttributeType defaultValue = default(TAttributeType))
        {
            if (Writing)
            {
                if (_attributes == null || _attributes.Count == 0)
                {
                    return defaultValue;
                }
                return _attributes.GetAttribute(attributeName, defaultValue);
            }
            else
            {
                // TODO: Get inherited attributes without allocating Attributes so we can use _attributes.
                return Attributes.GetInheritedAttribute(attributeName, inherited, defaultValue);
            }
        }

        /// <summary>
        /// Gets a collection of custom attributes
        /// </summary>
        public SvgCustomAttributeCollection CustomAttributes
        {
            get
            {
                if (this._customAttributes == null)
                {
                    this._customAttributes = new SvgCustomAttributeCollection(this);
                    //subscribe to attribute events
                    this._customAttributes.AttributeChanged += Attributes_AttributeChanged;
                }
                return this._customAttributes;
            }
        }

        /// <summary>
        /// Applies the required transforms to <see cref="ISvgRenderer"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> to be transformed.</param>
        protected internal virtual bool PushTransforms(ISvgRenderer renderer)
        {
            _graphicsTransform = renderer.Transform;
            _graphicsClip = renderer.GetClip();

            var transforms = Transforms;
            // Return if there are no transforms
            if (transforms == null || transforms.Count == 0)
                return true;

            using (var transformMatrix = transforms.GetMatrix())
            {
                using (var zeroMatrix = new Matrix(0f, 0f, 0f, 0f, 0f, 0f))
                    if (zeroMatrix.Equals(transformMatrix))
                        return false;

                using (var graphicsTransform = _graphicsTransform.Clone())
                {
                    graphicsTransform.Multiply(transformMatrix);
                    renderer.Transform = graphicsTransform;
                }
            }

            return true;
        }

        /// <summary>
        /// Removes any previously applied transforms from the specified <see cref="ISvgRenderer"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> that should have transforms removed.</param>
        protected internal virtual void PopTransforms(ISvgRenderer renderer)
        {
            renderer.Transform = _graphicsTransform;
            _graphicsTransform.Dispose();
            _graphicsTransform = null;
            renderer.SetClip(_graphicsClip);
            _graphicsClip = null;
        }

        /// <summary>
        /// Applies the required transforms to <see cref="ISvgRenderer"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> to be transformed.</param>
        void ISvgTransformable.PushTransforms(ISvgRenderer renderer)
        {
            PushTransforms(renderer);
        }

        /// <summary>
        /// Removes any previously applied transforms from the specified <see cref="ISvgRenderer"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> that should have transforms removed.</param>
        void ISvgTransformable.PopTransforms(ISvgRenderer renderer)
        {
            PopTransforms(renderer);
        }

        /// <summary>
        /// Gets or sets the element transforms.
        /// </summary>
        /// <value>The transforms.</value>
        [SvgAttribute("transform")]
        public SvgTransformCollection Transforms
        {
            get { return GetAttribute<SvgTransformCollection>("transform", false); }
            set
            {
                var old = Transforms;
                if (old != null)
                    old.TransformChanged -= Attributes_AttributeChanged;
                value.TransformChanged += Attributes_AttributeChanged;
                Attributes["transform"] = value;
            }
        }

        /// <summary>
        /// Transforms the given rectangle with the set transformation, if any.
        /// Can be applied to bounds calculated without considering the element transformation.
        /// </summary>
        /// <param name="bounds">The rectangle to be transformed.</param>
        /// <returns>The transformed rectangle, or the original rectangle if no transformation exists.</returns>
        protected RectangleF TransformedBounds(RectangleF bounds)
        {
            if (Transforms != null && Transforms.Count > 0)
            {
                using (var path = new GraphicsPath())
                using (var matrix = Transforms.GetMatrix())
                {
                    path.AddRectangle(bounds);
                    path.Transform(matrix);
                    return path.GetBounds();
                }
            }
            return bounds;
        }

        /// <summary>
        /// Gets or sets the ID of the element.
        /// </summary>
        /// <exception cref="SvgException">The ID is already used within the <see cref="SvgDocument"/>.</exception>
        [SvgAttribute("id")]
        public string ID
        {
            get { return GetAttribute<string>("id", false); }
            set { SetAndForceUniqueID(value, false); }
        }

        /// <summary>
        /// Gets or sets the space handling.
        /// </summary>
        /// <value>The space handling.</value>
        [SvgAttribute("space", SvgAttributeAttribute.XmlNamespace)]
        public virtual XmlSpaceHandling SpaceHandling
        {
            get { return GetAttribute("space", true, XmlSpaceHandling.Inherit); }
            set { Attributes["space"] = value; }
        }

        public void SetAndForceUniqueID(string value, bool autoForceUniqueID = true, Action<SvgElement, string, string> logElementOldIDNewID = null)
        {
            // Don't do anything if it hasn't changed
            if (string.Compare(ID, value) == 0)
            {
                return;
            }

            if (OwnerDocument != null)
            {
                OwnerDocument.IdManager.Remove(this);
            }

            Attributes["id"] = value;

            if (OwnerDocument != null)
            {
                OwnerDocument.IdManager.AddAndForceUniqueID(this, null, autoForceUniqueID, logElementOldIDNewID);
            }
        }

        /// <summary>
        /// Only used by the ID Manager
        /// </summary>
        /// <param name="newID"></param>
        internal void ForceUniqueID(string newID)
        {
            Attributes["id"] = newID;
        }

        /// <summary>
        /// Called by the underlying <see cref="SvgElement"/> when an element has been added to the
        /// <see cref="Children"/> collection.
        /// </summary>
        /// <param name="child">The <see cref="SvgElement"/> that has been added.</param>
        /// <param name="index">An <see cref="int"/> representing the index where the element was added to the collection.</param>
        protected virtual void AddElement(SvgElement child, int index)
        {
        }

        /// <summary>
        /// Fired when an Element was added to the children of this Element
        /// </summary>
        public event EventHandler<ChildAddedEventArgs> ChildAdded;

        /// <summary>
        /// Calls the <see cref="AddElement"/> method with the specified parameters.
        /// </summary>
        /// <param name="child">The <see cref="SvgElement"/> that has been added.</param>
        /// <param name="index">An <see cref="int"/> representing the index where the element was added to the collection.</param>
        internal void OnElementAdded(SvgElement child, int index)
        {
            this.AddElement(child, index);
            SvgElement sibling = null;
            if (this.HasChildren())
            {
                if (index < (Children.Count - 1))
                {
                    sibling = Children[index + 1];
                }
            }
            var handler = ChildAdded;
            if (handler != null)
            {
                handler(this, new ChildAddedEventArgs { NewChild = child, BeforeSibling = sibling });
            }
        }

        /// <summary>
        /// Called by the underlying <see cref="SvgElement"/> when an element has been removed from the
        /// <see cref="Children"/> collection.
        /// </summary>
        /// <param name="child">The <see cref="SvgElement"/> that has been removed.</param>
        protected virtual void RemoveElement(SvgElement child)
        {
        }

        /// <summary>
        /// Calls the <see cref="RemoveElement"/> method with the specified <see cref="SvgElement"/> as the parameter.
        /// </summary>
        /// <param name="child">The <see cref="SvgElement"/> that has been removed.</param>
        internal void OnElementRemoved(SvgElement child)
        {
            this.RemoveElement(child);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgElement"/> class.
        /// </summary>
        public SvgElement()
        {
            this._elementName = string.Empty;
#if !USE_SOURCE_GENERATORS
            //find svg attribute descriptions
            _svgPropertyAttributes = from PropertyDescriptor a in TypeDescriptor.GetProperties(this)
                                     let attribute = a.Attributes[typeof(SvgAttributeAttribute)] as SvgAttributeAttribute
                                     where attribute != null
                                     select new PropertyAttributeTuple { Property = a, Attribute = attribute };

            _svgEventAttributes = from EventDescriptor a in TypeDescriptor.GetEvents(this)
                                  let attribute = a.Attributes[typeof(SvgAttributeAttribute)] as SvgAttributeAttribute
                                  where attribute != null
                                  select new EventAttributeTuple { Event = a.ComponentType.GetField(a.Name, BindingFlags.Instance | BindingFlags.NonPublic), Attribute = attribute };
#endif
        }

        //dispatch attribute event
        void Attributes_AttributeChanged(object sender, AttributeEventArgs e)
        {
            OnAttributeChanged(e);
        }

        public virtual void InitialiseFromXML(XmlReader reader, SvgDocument document)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renders this element to the <see cref="ISvgRenderer"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> that the element should use to render itself.</param>
        public void RenderElement(ISvgRenderer renderer)
        {
            Render(renderer);
        }

        /// <summary>Derrived classes may decide that the element should not be written. For example, the text element shouldn't be written if it's empty.</summary>
        public virtual bool ShouldWriteElement()
        {
            //Write any element who has a name.
            return !string.IsNullOrEmpty(this.ElementName);
        }

        protected virtual void WriteStartElement(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(this.ElementName))
            {
                writer.WriteStartElement(this.ElementName, this.ElementNamespace);
            }

            this.WriteAttributes(writer);
        }

        protected virtual void WriteEndElement(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(this.ElementName))
            {
                writer.WriteEndElement();
            }
        }
        protected virtual void WriteAttributes(XmlWriter writer)
        {
            //properties
            var styles = WritePropertyAttributes(writer);

            //events
            if (AutoPublishEvents)
            {
#if USE_SOURCE_GENERATORS
                foreach (var property in this.GetProperties().Where(x => x.DescriptorType == DescriptorType.Event))
                {
                    var evt = property.GetValue(this);

                    //if someone has registered publish the attribute
                    if (evt != null && !string.IsNullOrEmpty(this.ID))
                    {
                        string evtValue = this.ID + "/" + property.AttributeName;
                        WriteAttributeString(writer, property.AttributeName, null, evtValue);
                    }
                }
#else
                foreach (var attr in _svgEventAttributes)
                {
                    var evt = attr.Event.GetValue(this);

                    //if someone has registered publish the attribute
                    if (evt != null && !string.IsNullOrEmpty(this.ID))
                    {
                        string evtValue = this.ID + "/" + attr.Attribute.Name;
                        WriteAttributeString(writer, attr.Attribute.Name, null, evtValue);
                    }
                }
#endif
            }

            //add the custom attributes
            var additionalStyleValue = string.Empty;
            if (this._customAttributes != null)
            {
                foreach (var item in this._customAttributes)
                {
                    if (item.Key.Equals("style") && styles != null && styles.Any())
                    {
                        additionalStyleValue = item.Value;
                        continue;
                    }
                    WriteAttributeString(writer, item.Key, null, item.Value);
                }
            }

            //write the style property
            if (styles != null && styles.Any())
            {
                var styleValues = styles.Select(s => s.Key + ":" + s.Value)
                    .Concat(Enumerable.Repeat(additionalStyleValue, 1));
                WriteAttributeString(writer, "style", null, string.Join(";", styleValues));
            }
        }

        private Dictionary<string, string> WritePropertyAttributes(XmlWriter writer)
        {
            var styles = _styles?.ToDictionary(x => x.Key, x => x.Value.Last().Value);

#if USE_SOURCE_GENERATORS
            var opacityAttributes = new List<ISvgPropertyDescriptor>();
            var opacityValues = new Dictionary<string, float>();

            try
            {
                Writing = true;

                foreach (var property in this.GetProperties())
                {
                    if (property.Converter == null)
                    {
                        continue;
                    }
                    if (property.Converter.CanConvertTo(typeof(string)))
                    {
                        if (property.AttributeName == "fill-opacity" || property.AttributeName == "stroke-opacity")
                        {
                            opacityAttributes.Add(property);
                            continue;
                        }

                        if (Attributes.ContainsKey(property.AttributeName))
                        {
                            var propertyValue = property.GetValue(this);

                            var forceWrite = false;
                            var writeStyle = property.AttributeName == "fill" || property.AttributeName == "stroke";

                            if (Parent != null)
                            {
                                if (writeStyle && propertyValue == SvgPaintServer.NotSet)
                                    continue;

                                object parentValue;
                                if (TryResolveParentAttributeValue(property.AttributeName, out parentValue))
                                {
                                    if ((parentValue == propertyValue)
                                        || ((parentValue != null) && parentValue.Equals(propertyValue)))
                                    {
                                        if (writeStyle)
                                            continue;
                                    }
                                    else
                                        forceWrite = true;
                                }
                            }

                            var hasOpacity = writeStyle;
                            if (hasOpacity)
                            {
                                if (propertyValue is SvgColourServer && ((SvgColourServer)propertyValue).Colour.A < 255)
                                {
                                    var opacity = ((SvgColourServer)propertyValue).Colour.A / 255f;
                                    opacityValues.Add(property.AttributeName + "-opacity", opacity);
                                }
                            }

#if NETFULL
                            var value = (string)property.Converter.ConvertTo(propertyValue, typeof(string));
#else
                            // dotnetcore throws exception if input is null
                            var value = propertyValue == null ? null : (string)property.Converter.ConvertTo(propertyValue, typeof(string));
#endif

                            if (propertyValue != null)
                            {
                                //Only write the attribute's value if it is not the default value, not null/empty, or we're forcing the write.
                                if (forceWrite || !string.IsNullOrEmpty(value))
                                {
                                    if (writeStyle)
                                    {
                                        if (styles == null)
                                        {
                                            styles = new Dictionary<string, string>();
                                        }
                                        styles[property.AttributeName] = value;
                                    }
                                    else
                                    {
                                        WriteAttributeString(writer, property.AttributeName, property.AttributeNamespace, value);
                                    }
                                }
                            }
                            else if (property.AttributeName == "fill") //if fill equals null, write 'none'
                            {
                                if (writeStyle)
                                {
                                    if (styles == null)
                                    {
                                        styles = new Dictionary<string, string>();
                                    }
                                    styles[property.AttributeName] = value;
                                }
                                else
                                {
                                    WriteAttributeString(writer, property.AttributeName, property.AttributeNamespace, value);
                                }
                            }
                        }
                    }
                }

                foreach (var property in opacityAttributes)
                {
                    var opacity = 1f;
                    var write = false;

                    var key = property.AttributeName;
                    if (opacityValues.ContainsKey(key))
                    {
                        opacity = opacityValues[key];
                        write = true;
                    }
                    if (Attributes.ContainsKey(key))
                    {
                        opacity *= (float)property.GetValue(this);
                        write = true;
                    }
                    if (write)
                    {
                        opacity = (float)Math.Round(opacity, 2, MidpointRounding.AwayFromZero);
                        var value = (string)property.Converter.ConvertTo(opacity, typeof(string));
                        if (!string.IsNullOrEmpty(value))
                            WriteAttributeString(writer, property.AttributeName, property.AttributeNamespace, value);
                    }
                }
            }
            finally
            {
                Writing = false;
            }
#else
            var opacityAttributes = new List<PropertyAttributeTuple>();
            var opacityValues = new Dictionary<string, float>();

            try
            {
                Writing = true;

                foreach (var attr in _svgPropertyAttributes)
                {
                    if (attr.Property.Converter.CanConvertTo(typeof(string)))
                    {
                        if (attr.Attribute.Name == "fill-opacity" || attr.Attribute.Name == "stroke-opacity")
                        {
                            opacityAttributes.Add(attr);
                            continue;
                        }

                        if (Attributes.ContainsKey(attr.Attribute.Name))
                        {
                            var propertyValue = attr.Property.GetValue(this);

                            var forceWrite = false;
                            var writeStyle = attr.Attribute.Name == "fill" || attr.Attribute.Name == "stroke";

                            if (Parent != null)
                            {
                                if (writeStyle && propertyValue == SvgPaintServer.NotSet)
                                    continue;

                                object parentValue;
                                if (TryResolveParentAttributeValue(attr.Attribute.Name, out parentValue))
                                {
                                    if ((parentValue == propertyValue)
                                        || ((parentValue != null) && parentValue.Equals(propertyValue)))
                                    {
                                        if (writeStyle)
                                            continue;
                                    }
                                    else
                                        forceWrite = true;
                                }
                            }

                            var hasOpacity = writeStyle;
                            if (hasOpacity)
                            {
                                if (propertyValue is SvgColourServer && ((SvgColourServer)propertyValue).Colour.A < 255)
                                {
                                    var opacity = ((SvgColourServer)propertyValue).Colour.A / 255f;
                                    opacityValues.Add(attr.Attribute.Name + "-opacity", opacity);
                                }
                            }

#if NETFULL
                            var value = (string)attr.Property.Converter.ConvertTo(propertyValue, typeof(string));
#else
                            // dotnetcore throws exception if input is null
                            var value = propertyValue == null ? null : (string)attr.Property.Converter.ConvertTo(propertyValue, typeof(string));
#endif

                            if (propertyValue != null)
                            {
                                //Only write the attribute's value if it is not the default value, not null/empty, or we're forcing the write.
                                if (forceWrite || !string.IsNullOrEmpty(value))
                                {
                                    if (writeStyle)
                                    {
                                        if (styles == null)
                                        {
                                            styles = new Dictionary<string, string>();
                                        }
                                        styles[attr.Attribute.Name] = value;
                                    }
                                    else
                                    {
                                        WriteAttributeString(writer, attr.Attribute.Name, attr.Attribute.NameSpace, value);
                                    }
                                }
                            }
                            else if (attr.Attribute.Name == "fill") //if fill equals null, write 'none'
                            {
                                if (writeStyle)
                                {
                                    if (styles == null)
                                    {
                                        styles = new Dictionary<string, string>();
                                    }
                                    styles[attr.Attribute.Name] = value;
                                }
                                else
                                {
                                    WriteAttributeString(writer, attr.Attribute.Name, attr.Attribute.NameSpace, value);
                                }
                            }
                        }
                    }
                }

                foreach (var attr in opacityAttributes)
                {
                    var opacity = 1f;
                    var write = false;

                    var key = attr.Attribute.Name;
                    if (opacityValues.ContainsKey(key))
                    {
                        opacity = opacityValues[key];
                        write = true;
                    }
                    if (Attributes.ContainsKey(key))
                    {
                        opacity *= (float)attr.Property.GetValue(this);
                        write = true;
                    }
                    if (write)
                    {
                        opacity = (float)Math.Round(opacity, 2, MidpointRounding.AwayFromZero);
                        var value = (string)attr.Property.Converter.ConvertTo(opacity, typeof(string));
                        if (!string.IsNullOrEmpty(value))
                            WriteAttributeString(writer, attr.Attribute.Name, attr.Attribute.NameSpace, value);
                    }
                }
            }
            finally
            {
                Writing = false;
            }
#endif
            return styles;
        }


        private void WriteAttributeString(XmlWriter writer, string name, string ns, string value)
        {
            if (!string.IsNullOrEmpty(ns) && ns != this.ElementNamespace)
            {
                // a namespace has been specified that is different from the enclosing element's namespace;
                // write the attribute with the corresponding namespace prefix
                string prefix = writer.LookupPrefix(ns);
                writer.WriteAttributeString(prefix, name, ns, value);
            }
            else
            {
                writer.WriteAttributeString(name, value);
            }
        }


        public bool AutoPublishEvents = true;

        private bool TryResolveParentAttributeValue(string attributeKey, out object parentAttributeValue)
        {
            parentAttributeValue = null;

            //attributeKey = char.ToUpper(attributeKey[0]) + attributeKey.Substring(1);

            var currentParent = Parent;
            var resolved = false;
            while (currentParent != null)
            {
                if (currentParent.Attributes.ContainsKey(attributeKey))
                {
                    resolved = true;
                    parentAttributeValue = currentParent.Attributes[attributeKey];
                    if (parentAttributeValue != null)
                        break;
                }
                currentParent = currentParent.Parent;
            }

            return resolved;
        }

        /// <summary>
        /// Write this SvgElement out using a given XmlWriter.
        /// </summary>
        /// <param name="writer">The XmlWriter to use.</param>
        /// <remarks>
        /// Recommendation is to create an XmlWriter by calling a factory method,<br/>
        /// e.g. <see cref="XmlWriter.Create(System.IO.Stream)"/>,
        /// as per <a href="https://docs.microsoft.com/dotnet/api/system.xml.xmltextwriter#remarks">Microsoft documentation</a>.<br/>
        /// <br/>
        /// However, unlike an <see cref="XmlTextWriter"/> created via 'new XmlTextWriter()',<br/>
        /// a factory-constructed XmlWriter will not flush output until it is closed<br/>
        /// (normally via a using statement), or unless the client explicitly calls <see cref="XmlWriter.Flush()"/>.
        /// </remarks>
        public virtual void Write(XmlWriter writer)
        {
            if (ShouldWriteElement())
            {
                this.WriteStartElement(writer);
                this.WriteChildren(writer);
                this.WriteEndElement(writer);
            }
        }

        protected virtual void WriteChildren(XmlWriter writer)
        {
            if (this.HasNodes())
            {
                SvgContentNode content;
                foreach (var node in this.Nodes)
                {
                    content = node as SvgContentNode;
                    if (content == null)
                    {
                        ((SvgElement)node).Write(writer);
                    }
                    else if (!string.IsNullOrEmpty(content.Content))
                    {
                        writer.WriteString(content.Content);
                    }
                }
            }
            else
            {
                //write the content
                if (!String.IsNullOrEmpty(this.Content))
                    writer.WriteString(this.Content);

                //write all children
                if (this.HasChildren())
                {
                    foreach (SvgElement child in this.Children)
                    {
                        child.Write(writer);
                    }
                }
            }
        }

        /// <summary>
        /// Renders the <see cref="SvgElement"/> and contents to the specified <see cref="ISvgRenderer"/> object.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> object to render to.</param>
        protected virtual void Render(ISvgRenderer renderer)
        {
            try
            {
                PushTransforms(renderer);
                RenderChildren(renderer);
            }
            finally
            {
                PopTransforms(renderer);
            }
        }

        /// <summary>
        /// Renders the children of this <see cref="SvgElement"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> to render the child <see cref="SvgElement"/>s to.</param>
        protected virtual void RenderChildren(ISvgRenderer renderer)
        {
            if (this.HasChildren())
            {
                foreach (var element in Children)
                {
                    element.Render(renderer);
                }
            }
        }

        /// <summary>
        /// Renders the <see cref="SvgElement"/> and contents to the specified <see cref="ISvgRenderer"/> object.
        /// </summary>
        /// <param name="renderer">The <see cref="ISvgRenderer"/> object to render to.</param>
        void ISvgElement.Render(ISvgRenderer renderer)
        {
            Render(renderer);
        }

        /// <summary>
        /// Recursive method to add up the paths of all children
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="path"></param>
        protected void AddPaths(SvgElement elem, GraphicsPath path)
        {
            if (!this.HasChildren())
            {
                return;
            }

            foreach (var child in elem.Children)
            {
                // Skip to avoid double calculate Symbol element
                // symbol element is only referenced by use element
                // So here we need to skip when it is directly considered
                if (child is SvgSymbol)
                    continue;

                if (child is SvgVisualElement)
                {
                    if (!(child is SvgGroup))
                    {
                        var childPath = ((SvgVisualElement)child).Path(null);
                        if (childPath != null)
                            using (childPath = (GraphicsPath)childPath.Clone())
                            {
                                if (child.Transforms != null)
                                    using (var matrix = child.Transforms.GetMatrix())
                                        childPath.Transform(matrix);

                                if (childPath.PointCount > 0)
                                    path.AddPath(childPath, false);
                            }
                    }
                }

                if (!(child is SvgPaintServer)) AddPaths(child, path);
            }
        }

        /// <summary>
        /// Recursive method to add up the paths of all children
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="renderer"></param>
        protected GraphicsPath GetPaths(SvgElement elem, ISvgRenderer renderer)
        {
            var ret = new GraphicsPath();

            if (!this.HasChildren())
            {
                return ret;
            }

            foreach (var child in elem.Children)
            {
                if (child is SvgVisualElement)
                {
                    if (child is SvgGroup)
                    {
                        var childPath = GetPaths(child, renderer);
                        if (childPath.PointCount > 0)
                        {
                            if (child.Transforms != null)
                                using (var matrix = child.Transforms.GetMatrix())
                                    childPath.Transform(matrix);

                            ret.AddPath(childPath, false);
                        }
                    }
                    else
                    {
                        var childPath = ((SvgVisualElement)child).Path(renderer);
                        childPath = childPath != null ? (GraphicsPath)childPath.Clone() : new GraphicsPath();

                        // Non-group element can have child element which we have to consider. i.e tspan in text element
                        if (child.HasChildren())
                        {
                            var descendantPath = GetPaths(child, renderer);
                            if (descendantPath.PointCount > 0)
                                childPath.AddPath(descendantPath, false);
                        }

                        if (childPath.PointCount > 0)
                        {
                            if (child.Transforms != null)
                                using (var matrix = child.Transforms.GetMatrix())
                                    childPath.Transform(matrix);

                            ret.AddPath(childPath, false);
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            return DeepCopy();
        }

        public abstract SvgElement DeepCopy();

        ISvgNode ISvgNode.DeepCopy()
        {
            return DeepCopy();
        }

        public virtual SvgElement DeepCopy<T>() where T : SvgElement, new()
        {
            var newObj = new T
            {
                Content = Content,
                ElementName = ElementName
            };

            //if (this.Parent != null)
            //    this.Parent.Children.Add(newObj);

            foreach (var attribute in Attributes)
            {
                var value = attribute.Value is ICloneable ? ((ICloneable)attribute.Value).Clone() : attribute.Value;
                newObj.Attributes.Add(attribute.Key, value);
            }

            if (HasChildren())
            {
                foreach (var child in Children)
                {
                    newObj.Children.Add(child.DeepCopy());
                }
            }

#if USE_SOURCE_GENERATORS
            foreach (var property in this.GetProperties().Where(x => x.DescriptorType == DescriptorType.Event))
            {
                var evt = property.GetValue(this);

                // if someone has registered also register here
                if (evt != null)
                {
                    if (property.AttributeName == "MouseDown")
                        newObj.MouseDown += delegate { };
                    else if (property.AttributeName == "MouseUp")
                        newObj.MouseUp += delegate { };
                    else if (property.AttributeName == "MouseOver")
                        newObj.MouseOver += delegate { };
                    else if (property.AttributeName == "MouseOut")
                        newObj.MouseOut += delegate { };
                    else if (property.AttributeName == "MouseMove")
                        newObj.MouseMove += delegate { };
                    else if (property.AttributeName == "MouseScroll")
                        newObj.MouseScroll += delegate { };
                    else if (property.AttributeName == "Click")
                        newObj.Click += delegate { };
                    else if (property.AttributeName == "Change") // text element
                        (newObj as SvgText).Change += delegate { };
                }
            }
#else
            foreach (var attr in _svgEventAttributes)
            {
                var evt = attr.Event.GetValue(this);

                // if someone has registered also register here
                if (evt != null)
                {
                    if (attr.Event.Name == "MouseDown")
                        newObj.MouseDown += delegate { };
                    else if (attr.Event.Name == "MouseUp")
                        newObj.MouseUp += delegate { };
                    else if (attr.Event.Name == "MouseOver")
                        newObj.MouseOver += delegate { };
                    else if (attr.Event.Name == "MouseOut")
                        newObj.MouseOut += delegate { };
                    else if (attr.Event.Name == "MouseMove")
                        newObj.MouseMove += delegate { };
                    else if (attr.Event.Name == "MouseScroll")
                        newObj.MouseScroll += delegate { };
                    else if (attr.Event.Name == "Click")
                        newObj.Click += delegate { };
                    else if (attr.Event.Name == "Change") // text element
                        (newObj as SvgText).Change += delegate { };
                }
            }
#endif
            if (this._customAttributes != null)
            {
                foreach (var attribute in this._customAttributes)
                {
                    newObj.CustomAttributes.Add(attribute.Key, attribute.Value);
                }
            }

            if (this._nodes != null)
            {
                foreach (var node in this._nodes)
                {
                    newObj.Nodes.Add(node.DeepCopy());
                }
            }

            return newObj;
        }

        /// <summary>
        /// Fired when an Atrribute of this Element has changed
        /// </summary>
        public event EventHandler<AttributeEventArgs> AttributeChanged;

        protected void OnAttributeChanged(AttributeEventArgs args)
        {
            var handler = AttributeChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Fired when an Atrribute of this Element has changed
        /// </summary>
        public event EventHandler<ContentEventArgs> ContentChanged;

        protected void OnContentChanged(ContentEventArgs args)
        {
            var handler = ContentChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #region graphical EVENTS

        /*
            onfocusin = "<anything>"
            onfocusout = "<anything>"
            onactivate = "<anything>"
            onclick = "<anything>"
            onmousedown = "<anything>"
            onmouseup = "<anything>"
            onmouseover = "<anything>"
            onmousemove = "<anything>"
            onmouseout = "<anything>"
         */

#if Net4
        /// <summary>
        /// Use this method to provide your implementation ISvgEventCaller which can register Actions
        /// and call them if one of the events occurs. Make sure, that your SvgElement has a unique ID.
        /// The SvgTextElement overwrites this and regsiters the Change event tor its text content.
        /// </summary>
        /// <param name="caller"></param>
        public virtual void RegisterEvents(ISvgEventCaller caller)
        {
            if (caller != null && !string.IsNullOrEmpty(this.ID))
            {
                var rpcID = this.ID + "/";

                caller.RegisterAction<float, float, int, int, bool, bool, bool, string>(rpcID + "onclick", CreateMouseEventAction(RaiseMouseClick));
                caller.RegisterAction<float, float, int, int, bool, bool, bool, string>(rpcID + "onmousedown", CreateMouseEventAction(RaiseMouseDown));
                caller.RegisterAction<float, float, int, int, bool, bool, bool, string>(rpcID + "onmouseup", CreateMouseEventAction(RaiseMouseUp));
                caller.RegisterAction<float, float, int, int, bool, bool, bool, string>(rpcID + "onmousemove", CreateMouseEventAction(RaiseMouseMove));
                caller.RegisterAction<float, float, int, int, bool, bool, bool, string>(rpcID + "onmouseover", CreateMouseEventAction(RaiseMouseOver));
                caller.RegisterAction<float, float, int, int, bool, bool, bool, string>(rpcID + "onmouseout", CreateMouseEventAction(RaiseMouseOut));
                caller.RegisterAction<int, bool, bool, bool, string>(rpcID + "onmousescroll", OnMouseScroll);
            }
        }

        /// <summary>
        /// Use this method to provide your implementation ISvgEventCaller to unregister Actions
        /// </summary>
        /// <param name="caller"></param>
        public virtual void UnregisterEvents(ISvgEventCaller caller)
        {
            if (caller != null && !string.IsNullOrEmpty(this.ID))
            {
                var rpcID = this.ID + "/";

                caller.UnregisterAction(rpcID + "onclick");
                caller.UnregisterAction(rpcID + "onmousedown");
                caller.UnregisterAction(rpcID + "onmouseup");
                caller.UnregisterAction(rpcID + "onmousemove");
                caller.UnregisterAction(rpcID + "onmousescroll");
                caller.UnregisterAction(rpcID + "onmouseover");
                caller.UnregisterAction(rpcID + "onmouseout");
            }
        }
#endif

        [SvgAttribute("onclick")]
        public event EventHandler<MouseArg> Click;

        [SvgAttribute("onmousedown")]
        public event EventHandler<MouseArg> MouseDown;

        [SvgAttribute("onmouseup")]
        public event EventHandler<MouseArg> MouseUp;

        [SvgAttribute("onmousemove")]
        public event EventHandler<MouseArg> MouseMove;

        [SvgAttribute("onmousescroll")]
        public event EventHandler<MouseScrollArg> MouseScroll;

        [SvgAttribute("onmouseover")]
        public event EventHandler<MouseArg> MouseOver;

        [SvgAttribute("onmouseout")]
        public event EventHandler<MouseArg> MouseOut;

#if Net4
        protected Action<float, float, int, int, bool, bool, bool, string> CreateMouseEventAction(Action<object, MouseArg> eventRaiser)
        {
            return (x, y, button, clickCount, altKey, shiftKey, ctrlKey, sessionID) =>
                eventRaiser(this, new MouseArg { x = x, y = y, Button = button, ClickCount = clickCount, AltKey = altKey, ShiftKey = shiftKey, CtrlKey = ctrlKey, SessionID = sessionID });
        }
#endif

        //click
        protected void RaiseMouseClick(object sender, MouseArg e)
        {
            var handler = Click;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        //down
        protected void RaiseMouseDown(object sender, MouseArg e)
        {
            var handler = MouseDown;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        //up
        protected void RaiseMouseUp(object sender, MouseArg e)
        {
            var handler = MouseUp;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected void RaiseMouseMove(object sender, MouseArg e)
        {
            var handler = MouseMove;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        //over
        protected void RaiseMouseOver(object sender, MouseArg args)
        {
            var handler = MouseOver;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        //out
        protected void RaiseMouseOut(object sender, MouseArg args)
        {
            var handler = MouseOut;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        //scroll
        protected void OnMouseScroll(int scroll, bool ctrlKey, bool shiftKey, bool altKey, string sessionID)
        {
            RaiseMouseScroll(this, new MouseScrollArg { Scroll = scroll, AltKey = altKey, ShiftKey = shiftKey, CtrlKey = ctrlKey, SessionID = sessionID });
        }

        protected void RaiseMouseScroll(object sender, MouseScrollArg e)
        {
            var handler = MouseScroll;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        #endregion graphical EVENTS
    }

    public class SVGArg : EventArgs
    {
        public string SessionID;
    }

    /// <summary>
    /// Describes the Attribute which was set
    /// </summary>
    public class AttributeEventArgs : SVGArg
    {
        public string Attribute;
        public object Value;
    }

    /// <summary>
    /// Content of this whas was set
    /// </summary>
    public class ContentEventArgs : SVGArg
    {
        public string Content;
    }

    /// <summary>
    /// Describes the Attribute which was set
    /// </summary>
    public class ChildAddedEventArgs : SVGArg
    {
        public SvgElement NewChild;
        public SvgElement BeforeSibling;
    }

#if Net4
    //deriving class registers event actions and calls the actions if the event occurs
    public interface ISvgEventCaller
    {
        void RegisterAction(string rpcID, Action action);
        void RegisterAction<T1>(string rpcID, Action<T1> action);
        void RegisterAction<T1, T2>(string rpcID, Action<T1, T2> action);
        void RegisterAction<T1, T2, T3>(string rpcID, Action<T1, T2, T3> action);
        void RegisterAction<T1, T2, T3, T4>(string rpcID, Action<T1, T2, T3, T4> action);
        void RegisterAction<T1, T2, T3, T4, T5>(string rpcID, Action<T1, T2, T3, T4, T5> action);
        void RegisterAction<T1, T2, T3, T4, T5, T6>(string rpcID, Action<T1, T2, T3, T4, T5, T6> action);
        void RegisterAction<T1, T2, T3, T4, T5, T6, T7>(string rpcID, Action<T1, T2, T3, T4, T5, T6, T7> action);
        void RegisterAction<T1, T2, T3, T4, T5, T6, T7, T8>(string rpcID, Action<T1, T2, T3, T4, T5, T6, T7, T8> action);
        void UnregisterAction(string rpcID);
    }
#endif

    /// <summary>
    /// Represents the state of the mouse at the moment the event occured.
    /// </summary>
    public class MouseArg : SVGArg
    {
        public float x;
        public float y;

        /// <summary>
        /// 1 = left, 2 = middle, 3 = right
        /// </summary>
        public int Button;

        /// <summary>
        /// Amount of mouse clicks, e.g. 2 for double click
        /// </summary>
        public int ClickCount = -1;

        /// <summary>
        /// Alt modifier key pressed
        /// </summary>
        public bool AltKey;

        /// <summary>
        /// Shift modifier key pressed
        /// </summary>
        public bool ShiftKey;

        /// <summary>
        /// Control modifier key pressed
        /// </summary>
        public bool CtrlKey;
    }

    /// <summary>
    /// Represents a string argument
    /// </summary>
    public class StringArg : SVGArg
    {
        public string s;
    }

    public class MouseScrollArg : SVGArg
    {
        public int Scroll;

        /// <summary>
        /// Alt modifier key pressed
        /// </summary>
        public bool AltKey;

        /// <summary>
        /// Shift modifier key pressed
        /// </summary>
        public bool ShiftKey;

        /// <summary>
        /// Control modifier key pressed
        /// </summary>
        public bool CtrlKey;
    }

    public interface ISvgNode
    {
        string Content { get; }

        /// <summary>
        /// Create a deep copy of this <see cref="ISvgNode"/>.
        /// </summary>
        /// <returns>A deep copy of this <see cref="ISvgNode"/></returns>
        ISvgNode DeepCopy();
    }

    /// <summary>This interface mostly indicates that a node is not to be drawn when rendering the SVG.</summary>
    public interface ISvgDescriptiveElement
    {
    }

    internal interface ISvgElement
    {
        SvgElement Parent { get; }
        SvgElementCollection Children { get; }
        IList<ISvgNode> Nodes { get; }

        void Render(ISvgRenderer renderer);
    }
}
