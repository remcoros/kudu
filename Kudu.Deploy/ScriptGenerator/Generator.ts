///<reference path='ensure.ts'/>

function generateDeploymentScript(repositoryRoot: string, projectType: string, projectPath: string, solutionPath: string) {
    Ensure.argNotNull(projectType, "projectType");

    projectType = projectType.toUpperCase();
    if (projectType == "WAP") {
        generateWapDeploymentScript(repositoryRoot, projectPath, solutionPath);
    }
    else if (projectType == "WEBSITE") {
        generateWebSiteDeploymentScript(repositoryRoot, solutionPath);
    }
    else {
        throw new Error("Invalid project type received: " + projectType);
    }
}

function generateWapDeploymentScript(repositoryRoot: string, projectPath: string, solutionPath: string) {
    Ensure.argNotNull(projectPath, "projectPath");

    var relativeProjectPath = pathUtil.relative(repositoryRoot, projectPath);
    var relativeSolutionPath = pathUtil.relative(repositoryRoot, solutionPath);

    var msbuildArguments = "\"%DEPLOYMENT_SOURCE%\\" + relativeProjectPath + "\" /nologo /verbosity:m /t:pipelinePreDeployCopyAllFilesToOneFolder /p:_PackageTempDir=\"%TEMPORARY_PATH%\";AutoParameterizationWebConfigConnectionStrings=false;Configuration=Release";
    if (solutionPath != null) {
        msbuildArguments += " /p:SolutionDir=\"%DEPLOYMENT_SOURCE%\\" + relativeSolutionPath + "\"";
    }

    generateDotNetDeploymentScript("deploy.wap.template", msbuildArguments);
}

function generateWebSiteDeploymentScript(repositoryRoot: string, solutionPath: string) {
    if (solutionPath == null) {
        throw new Error("The solution file path is required for .NET web site deployment script");
    }

    var relativeSolutionPath = pathUtil.relative(repositoryRoot, solutionPath);

    var msbuildArguments = "\"%DEPLOYMENT_SOURCE%\\" + relativeSolutionPath + "\" /verbosity:m /nologo";
    generateDotNetDeploymentScript("deploy.website.template", msbuildArguments);
}

function generateDotNetDeploymentScript(templatePath: string, msbuildArguments: string) {
    Ensure.argNotNull(templatePath, "templatePath");

    var winDir = process.env["WINDIR"];
    var msbuildPath = winDir + "\\Microsoft.NET\\Framework\\v4.0.30319";
    /*if (!fs.existsSync(msbuildPath)) {
        msbuildPath = winDir + "\\Microsoft.NET\\Framework64\\v4.0.30319";
    }*/

    var prefixContent: string = fs.readFileSync("deploy.prefix.template", "utf8");
    var specificTemplateContent = fs.readFileSync(templatePath, "utf8");
    var postfixContent = fs.readFileSync("deploy.postfix.template", "utf8");

    var templateContent = prefixContent + specificTemplateContent + postfixContent;
    templateContent =
        templateContent.replace("{MSBuildArguments}", msbuildArguments)
                       .replace("{MSBuildPath}", msbuildPath);

    fs.writeFileSync("deploy.cmd", templateContent);
}
