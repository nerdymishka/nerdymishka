
## Functions / Commands

- **Find-GitProjectRoot** will start with the current working directory and traverse
  up the tree to find the nearest parent folder that holds a ".git" sub folder.
- **Get-BuildArtifactsDirectory** will return the artifacts directory.  
  - If NM_BUILD_ARTIFACTS_DIR is set, it will return that value.
  - If the Azure DevOps folder is detected, it will return that.
  - If it can't find any values, it will attempt to find the root project
    folder that contains a .git folder and append "/artifacts" to it. 

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
- **NM_BUILD_ARTIFACTS_DIR** The build artifacts directory. If the value is not
  set Get-BuildArtifactsDirectory will set the default to the project "root/artifacts"
  where root is parent folder of a .git folder.