PHONY: binding

binding: _clean _build
	# Make binding

_clean:
	# Clean target

_build:
	msbuild /target:PhotectorSharp