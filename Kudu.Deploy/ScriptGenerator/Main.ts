///<reference path='generator.ts'/>

var yanop = require("yanop");

var opts = yanop.simple({
    repositoryRoot: {
        type: yanop.scalar,
        short: 'r',
        description: 'Root path for the repository',
        required: true
    },
    projectType: {
        type: yanop.scalar,
        short: 't',
        description: 'The project type can be: wap, website, nodejs, other',
        required: true
    },
    projectFile: {
        type: yanop.scalar,
        short: 'p',
        description: 'Project file path (only for wap)',
    },
    solutionFile: {
        type: yanop.scalar,
        short: 's',
        description: 'Solution file path (only for wap or website)',
    }
}, "Custom deployment script generator");

try {
    generateDeploymentScript(opts.repositoryRoot, opts.projectType, opts.projectFile, opts.solutionFile);
}
catch (e) {
    console.log("Error: " + e.message);
}
