# SVG Benchmarks

### Run `All` Benchmarks

```
dotnet run -c Release -f netcoreapp3.1 -- -f '*'
```

### Run `SvgDocument` Benchmarks

```
dotnet run -c Release -f netcoreapp3.1 -- -f '*SvgDocument_*'
```

### Run `ToStringBenchmarks` Benchmarks

```
dotnet run -c Release -f netcoreapp3.1 -- -f '*ToStringBenchmarks*'
```

### Run `SvgPathBuilder` Benchmarks

```
dotnet run -c Release -f netcoreapp3.1 -- -f '*SvgPathBuilder_*'
```

### Run `CoordinateParser` Benchmarks

```
dotnet run -c Release -f netcoreapp3.1 -- -f '*CoordinateParser_*'
```

### Run `SvgTransformConverter` Benchmarks

```
dotnet run -c Release -f netcoreapp3.1 -- -f '*SvgTransformConverter_*'
```

### Run `SvgUnitConverter` Benchmarks

```
dotnet run -c Release -f netcoreapp3.1 -- -f '*SvgUnitConverter_*'
```

### Run `SvgUnitCollectionConverter` and `SvgStrokeDashArrayConverter` Benchmarks

```
dotnet run -c Release -f netcoreapp3.1 -- -f '*SvgUnitCollectionConverter_*'

### Run `SvgNumberCollectionConverter` Benchmarks

```
dotnet run -c Release -f netcoreapp3.1 -- -f '*SvgNumberCollectionConverter_*'
```

### Run `SvgPointCollectionConverter` Benchmarks

```
dotnet run -c Release -f netcoreapp3.1 -- -f '*SvgPointCollectionConverter_*'
```
