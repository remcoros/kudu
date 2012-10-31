///<reference path='ensure.ts'/>

function generateDeploymentScript(projectType: string, projectPath: string, solutionPath: string) {
    Ensure.argNotNull(projectType, "projectType");

    projectType = projectType.toUpperCase();
    if (projectType == "WAP") {
        generateWapDeploymentScript(projectPath, solutionPath);
    }
    else if (projectType == "WEBSITE") {
        generateWebSiteDeploymentScript(solutionPath);
    }
    else {
        throw new Error("Invalid project type received: " + projectType);
    }
}

function generateWapDeploymentScript(projectPath: string, solutionPath: string) {
    Ensure.argNotNull(projectPath, "projectPath");

    var msbuildArguments = "\"" + projectPath + "\" /nologo /verbosity:m /t:pipelinePreDeployCopyAllFilesToOneFolder /p:_PackageTempDir=\"%TEMPORARY_PATH%\";AutoParameterizationWebConfigConnectionStrings=false;Configuration=Release /p:SolutionDir=\"%SOLUTION_PATH%\"";
    if (solutionPath != null) {
        msbuildArguments += " /p:SolutionDir=\"" + solutionPath + "\"";
    }

    generateDotNetDeploymentScript("deploy.wap.template", msbuildArguments);
}

function generateWebSiteDeploymentScript(solutionPath: string) {
    if (solutionPath == null) {
        throw new Error("The solution file path is required for .NET web site deployment script");
    }

    var msbuildArguments = "\"%SOLUTION_PATH%\" /verbosity:m /nologo";
    generateDotNetDeploymentScript("deploy.website.template", msbuildArguments);
}

function generateDotNetDeploymentScript(templatePath: string, msbuildArguments: string) {
    Ensure.argNotNull(templatePath, "templatePath");

    var winDir = process.env["WINDIR"];
    var msbuildPath = winDir + "\Microsoft.NET\Framework\v4.0.30319";
    /*if (!fs.existsSync(msbuildPath)) {
        msbuildPath = winDir + "\Microsoft.NET\Framework64\v4.0.30319";
    }*/

    var templateContent = fs.readFileSync(templatePath, "utf8");

    templateContent =
        templateContent.replace("{MSBuildArguments}", msbuildArguments)
                       .replace("{MSBuildPath}", msbuildPath);

    fs.writeFileSync("deploy.cmd", templateContent);
}
