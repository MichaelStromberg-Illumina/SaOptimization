# SaOptimization
The following projects are included in this solution:

 - `Benchmarks`
	 - this will be used to test newer versions of the SA files against the version currently used in develop
	 - the `PreloadBaseline` project contains the legacy SA code

 - `Compression`
	 - lightweight version of our normal compression library. Contains zstd (normal & dictionary verions)

 - `CreateGnomadVersion1`
	 - creates the v1 SA files for gnomAD
	 - uses the `Version1` library

 - `Preloader`
	 - library that reads a list of positions to preload
