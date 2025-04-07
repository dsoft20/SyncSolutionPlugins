
# Sync solution plugins for PACX
A plugin for PACX that syncs the plugin assemblies and plugin packages loaded on a solution to the local built version.
The plugin can, if necessary, rebuild the entire project or the single project specific to a plugin assembly/package.


# Installation
Download the release or clone the repository and build it. 
A NuGet package will be available at a later date.
### Built version (Downloaded from release or manually built)
The build dll must be placed in the following folder`C:\Users\<username>\AppData\Local\Greg.Xrm.Command\SyncSolutionPlugins`
### NuGet Package
Not yet available
# Usage
The plugin can be executed from the command line or from the PACX UI.
`pacx dstPlugins syncsolutionplugins -s test`
or the shortened version
`pacx dst sync -s test`
# Arguments
|Long name|Short name | Required? | Description | Default value | Valid values |
|--|--|--|--|--|--|
|solution| s|N|The name of the solution. If not provided the current solution is used|- | String|
|path| p|N|The path of the source folder. If not provided the directory where pacx is executed will be used|- | String|
|allowpartialsync| aps|N|Continue execution if an error occours while updating a plugin assembly|False | Bool|
|preview| pr|N|Shows a preview of the execution. No changes to the system will be made. |False | Bool|
|rebuild| r|N|[Experimental] Rebuild the plugin project|False | Bool|
|rebuildall| ra|N|[Experimental] Rebuild all project|False | Bool|
|rebuildlogonsuccess| rl|N|[Experimental] Show log of rebuild proecesses even when the process succeds|False | Bool|
