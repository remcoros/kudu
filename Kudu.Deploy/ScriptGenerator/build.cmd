@echo off

echo Installing yanop package
call npm install yanop

echo Building JavaScript from TypeScript files
tsc --module node --out scriptGenerator.js main.ts
