using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Svg
{
    /// <summary>
    /// Provides methods to ensure element ID's are valid and unique.
    /// </summary>
    public class SvgElementIdManager
    {
        private SvgDocument _document;
        private Dictionary<string, SvgElement> _idValueMap;

        /// <summary>
        /// Retrieves the <see cref="SvgElement"/> with the specified ID.
        /// </summary>
        /// <param name="id">A <see cref="string"/> containing the ID of the element to find.</param>
        /// <returns>An <see cref="SvgElement"/> of one exists with the specified ID; otherwise false.</returns>
        public virtual SvgElement GetElementById(string id)
        {
            id = GetUrlString(id);
            if (id.StartsWith("#"))
            {
                id = id.Substring(1);
            }

            SvgElement element = null;
            this._idValueMap.TryGetValue(id, out element);

            return element;
        }

        public virtual SvgElement GetElementById(Uri uri)
        {
            var urlString = GetUrlString(uri.ToString());

            if (!urlString.StartsWith("#"))
            {
                var index = urlString.LastIndexOf('#');
                var fragment = urlString.Substring(index);

                uri = new Uri(urlString.Remove(index, fragment.Length), UriKind.RelativeOrAbsolute);

                if (!uri.IsAbsoluteUri && _document.BaseUri != null)
                    uri = new Uri(_document.BaseUri, uri);


                if (!SvgDocument.ResolveExternalElements.AllowsResolving(uri))
                {
                    Trace.TraceWarning("Trying to resolve element by ID from '{0}', but resolving external resources of that type is disabled.", uri);
                    return null;
                }

                if (uri.IsAbsoluteUri)
                {
                    if (uri.IsFile)
                    {
                        var doc = SvgDocument.Open<SvgDocument>(uri.LocalPath);
                        return doc.IdManager.GetElementById(fragment);
                    }
                    else if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                    {
                        var httpRequest = WebRequest.Create(uri);
                        using (var webResponse = httpRequest.GetResponse())
                        {
                            var doc = SvgDocument.Open<SvgDocument>(webResponse.GetResponseStream());
                            return doc.IdManager.GetElementById(fragment);
                        }
                    }
                    else
                        throw new NotSupportedException();
                }
            }

            return GetElementById(urlString);
        }

        private static string GetUrlString(string url)
        {
            url = url.Trim();
            if (url.StartsWith("url(", StringComparison.OrdinalIgnoreCase) && url.EndsWith(")"))
            {
                url = new StringBuilder(url).Remove(url.Length - 1, 1).Remove(0, 4).ToString().Trim();

                if ((url.StartsWith("\"") && url.EndsWith("\"")) || (url.StartsWith("'") && url.EndsWith("'")))
                    url = new StringBuilder(url).Remove(url.Length - 1, 1).Remove(0, 1).ToString().Trim();
            }
            return url;
        }

        /// <summary>
        /// Adds the specified <see cref="SvgElement"/> for ID management.
        /// </summary>
        /// <param name="element">The <see cref="SvgElement"/> to be managed.</param>
        public virtual void Add(SvgElement element)
        {
            AddAndForceUniqueID(element, null, false);
        }

        /// <summary>
        /// Adds the specified <see cref="SvgElement"/> for ID management.
        /// And can auto fix the ID if it already exists or it starts with a number.
        /// </summary>
        /// <param name="element">The <see cref="SvgElement"/> to be managed.</param>
        /// <param name="sibling">Not used.</param>
        /// <param name="autoForceUniqueID">Pass true here, if you want the ID to be fixed</param>
        /// <param name="logElementOldIDNewID">If not null, the action is called before the id is fixed</param>
        /// <returns>true, if ID was altered</returns>
        public virtual bool AddAndForceUniqueID(SvgElement element, SvgElement sibling, bool autoForceUniqueID = true, Action<SvgElement, string, string> logElementOldIDNewID = null)
        {
            var result = false;
            if (!string.IsNullOrEmpty(element.ID))
            {
                var newID = this.EnsureValidId(element.ID, autoForceUniqueID);
                if (autoForceUniqueID && newID != element.ID)
                {
                    if (logElementOldIDNewID != null)
                        logElementOldIDNewID(element, element.ID, newID);
                    element.ForceUniqueID(newID);
                    result = true;
                }
                this._idValueMap.Add(element.ID, element);
            }

            OnAdded(element);
            return result;
        }

        /// <summary>
        /// Removed the specified <see cref="SvgElement"/> from ID management.
        /// </summary>
        /// <param name="element">The <see cref="SvgElement"/> to be removed from ID management.</param>
        public virtual void Remove(SvgElement element)
        {
            if (!string.IsNullOrEmpty(element.ID))
            {
                this._idValueMap.Remove(element.ID);
            }

            OnRemoved(element);
        }

        /// <summary>
        /// Ensures that the specified ID is unique within the containing <see cref="SvgDocument"/>.
        /// </summary>
        /// <param name="id">A <see cref="string"/> containing the ID to validate.</param>
        /// <param name="autoForceUniqueID">Creates a new unique id <see cref="string"/>.</param>
        /// <exception cref="SvgException">
        /// <para>An element with the same ID already exists within the containing <see cref="SvgDocument"/>.</para>
        /// </exception>
        public string EnsureValidId(string id, bool autoForceUniqueID = false)
        {

            if (string.IsNullOrEmpty(id))
            {
                return id;
            }

            if (this._idValueMap.ContainsKey(id))
            {
                if (autoForceUniqueID)
                {
                    var match = regex.Match(id);

                    int number;
                    if (match.Success && int.TryParse(match.Value.Substring(1), out number))
                    {
                        id = regex.Replace(id, "#" + (number + 1));
                    }
                    else
                    {
                        id += "#1";
                    }

                    return EnsureValidId(id, true);
                }
                throw new SvgIDExistsException("An element with the same ID already exists: '" + id + "'.");
            }

            return id;
        }
        private static readonly Regex regex = new Regex(@"#\d+$");

        /// <summary>
        /// Initialises a new instance of an <see cref="SvgElementIdManager"/>.
        /// </summary>
        /// <param name="document">The <see cref="SvgDocument"/> containing the <see cref="SvgElement"/>s to manage.</param>
        public SvgElementIdManager(SvgDocument document)
        {
            this._document = document;
            this._idValueMap = new Dictionary<string, SvgElement>();
        }

        public event EventHandler<SvgElementEventArgs> ElementAdded;
        public event EventHandler<SvgElementEventArgs> ElementRemoved;

        protected void OnAdded(SvgElement element)
        {
            var handler = ElementAdded;
            if (handler != null)
            {
                handler(this._document, new SvgElementEventArgs { Element = element });
            }
        }

        protected void OnRemoved(SvgElement element)
        {
            var handler = ElementRemoved;
            if (handler != null)
            {
                handler(this._document, new SvgElementEventArgs { Element = element });
            }
        }

    }

    public class SvgElementEventArgs : EventArgs
    {
        public SvgElement Element;
    }
}
