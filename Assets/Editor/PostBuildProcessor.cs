using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class PostBuildProcessor
{
    [PostProcessBuild(9999)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.iOS)
        {
            string frameworkPath = Path.Combine(pathToBuiltProject, "Frameworks/UnityFramework.framework/Frameworks");
            if (Directory.Exists(frameworkPath))
            {
                Directory.Delete(frameworkPath, true);
            }

            string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);
            string t = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(t, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            pbxProject.WriteToFile(projectPath);
        }
    }

}