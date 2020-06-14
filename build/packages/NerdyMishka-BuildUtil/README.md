## Environment Variables

- **NM_BUILD_DB** Build database location. The default value if this variable
  is empty is `$HOME/.nerdymishka/build/build.db`
- **NM_BUILD_DEFINITIONNAME** The name of the build definition. If value is not
  set it will try to get the name from the native CI tool or use the git
  repo to get the project name and append _Local to it.
- **NM_BUILD_NUMBER_FORMAT** The string format used to create a new
  build number (label) for a local build. If this is not specified, the value
  defaults to `$(Build.DefinitionName)_$(Date:yyyyMMdd).$(Rev:rr)`
- **NM_BUILD_NUMBER** The build number or label for the current build. If
  this is not set or can not find `$ENV:BUILD_NUMBER` it will generate a
  value use the NM_BUILD_NUMBER_FORMAT.
