const esbuild = require('esbuild');
const path = require('path');

const outPath = path.resolve(process.env.OUT_DIR);
const outfile = path.join(outPath, 'editor.bundle.js');

/** @type {import('esbuild').BuildOptions} */
const buildOptions = {
    entryPoints: [path.resolve(__dirname, 'editor.ts')],
    bundle: true,
    outfile: outfile,
    format: 'esm',
    target: 'es2020',
    sourcemap: true,
};

esbuild.build(buildOptions).then(() => {
    console.log(`[esbuild] Successfully bundled editor to ${outfile}`);
}).catch((err) => {
    console.error(err);
    process.exit(1);
});
