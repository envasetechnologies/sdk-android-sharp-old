PHONY: binding _clean _build _copy_dlls

OUTPUT_DIR := $(shell pwd)/Release/

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
	if [[ -d "${OUTPUT_DIR}" ]]; then rm -Rf "${OUTPUT_DIR}"; fi
	if [[ ! -d "${OUTPUT_DIR}" ]]; then mkdir "${OUTPUT_DIR}"; fi
	find "Demo/bin/Debug/" -iname \*.dll -exec cp {} "${OUTPUT_DIR}" \;
	rm "${OUTPUT_DIR}Demo.dll"
