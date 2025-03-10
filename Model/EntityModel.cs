namespace SyncSolutionPlugins.Model
{
    public static class EntityModel
    {
        public static class pluginassembly
        {
            public static string pluginassemblyid => nameof(pluginassemblyid);
            public static string content => nameof(content);
            public static string name => nameof(name);
            public static string packageid => nameof(packageid);
        }

        public static class solution
        {
            public static string solutionid => nameof(solutionid);
            public static string uniquename => nameof(uniquename);
            public static string ismanaged => nameof(ismanaged);
        }

        public static class solutioncomponent
        {
            public static string solutioncomponentid => nameof(solutioncomponentid);
            public static string componenttype => nameof(componenttype);
            public static string objectid => nameof(objectid);
            public static string solutionid => nameof(solutionid);
        }

        public static class pluginpackage
        {
            public static string pluginpackageid => nameof(pluginpackageid);
            public static string name => nameof(name);
            public static string uniquename => nameof(uniquename);
            public static string solutionid => nameof(solutionid);
            public static string content => nameof(content);
        }
    }
}