@echo off

echo Installing typescript
call npm install -g typescript

echo Building JavaScript from TypeScript files
tsc --module node --out smartCopy.js main.ts
