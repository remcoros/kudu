///<reference path='ensure.ts'/>

function generateDeploymentScript(projectType: string, projectPath: string, solutionPath: string) {
    Ensure.argNotNull(projectType, "projectType");

    projectType = projectType.toUpperCase();
    if (projectType == "WAP") {
        generateWapDeploymentScript(projectPath, solutionPath);
    }
    else {
        throw new Error("Invalid project type received: " + projectType);
    }
}

function generateWapDeploymentScript(projectPath: string, solutionPath: string) {
    Ensure.argNotNull(projectPath, "projectPath");
    Ensure.argNotNull(solutionPath, "solutionPath");

    var winDir = process.env["WINDIR"];
    var msbuildPath = winDir + "\Microsoft.NET\Framework\v4.0.30319";
    if (!fs.existsSync(msbuildPath)) {
        msbuildPath = winDir + "\Microsoft.NET\Framework64\v4.0.30319";
    }

    var templateContent = fs.readFileSync("deploy.wap.template", "utf8");
    var msbuildArguments = "/p:SolutionDir=\"%SOLUTION_PATH%\""

    if (solutionPath != null) {
        msbuildArguments += " /p:SolutionDir=\"%SOLUTION_PATH%\"";
    }

    templateContent =
        templateContent.replace("{ProjectPath}", projectPath)
                       .replace("{SolutionPath}", solutionPath)
                       .replace("{MSBuildArguments}", msbuildArguments)
                       .replace("{MSBuildPath}", msbuildPath);

    fs.writeFileSync("deploy.cmd", templateContent);
}
