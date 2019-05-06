PHONY: binding _clean _build _copy_dlls

binding: _clean _build _copy_dlls
	# Make binding

_clean:
	# Clean all targets
	msbuild /t:Clean

_build:
	# Build all targets if needed
	msbuild /t:Build

_copy_dlls:
	# Copy all DLL's to the Release directory & afterwards remove Demo.dll
	rm Release/**
	find Demo/bin/Debug/ -type f -name '*.dll' | xargs -J % cp % Release/
	rm Release/Demo.dll
