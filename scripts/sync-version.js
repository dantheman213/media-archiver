// Syncs version from package.json to Cargo.toml and tauri.conf.json.
// Usage: node scripts/sync-version.js [version]
//   If version is omitted, reads from package.json.
import { readFileSync, writeFileSync } from 'node:fs';

const version = process.argv[2] || JSON.parse(readFileSync('package.json', 'utf8')).version;
console.log(`Syncing version: ${version}`);

// Update src-tauri/Cargo.toml — only the [package] section's version line
const cargoPath = 'src-tauri/Cargo.toml';
const cargo = readFileSync(cargoPath, 'utf8')
  .replace(/^version = ".*?"/m, `version = "${version}"`);
writeFileSync(cargoPath, cargo);
console.log(`  Updated ${cargoPath}`);

// Update src-tauri/tauri.conf.json
const tauriConfPath = 'src-tauri/tauri.conf.json';
const tauriConf = JSON.parse(readFileSync(tauriConfPath, 'utf8'));
tauriConf.version = version;
writeFileSync(tauriConfPath, JSON.stringify(tauriConf, null, 2) + '\n');
console.log(`  Updated ${tauriConfPath}`);
