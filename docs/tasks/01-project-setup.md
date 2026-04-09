# Task 1: Project Setup

## Objective
Initialize the Media Archiver project with the required technology stack.

## Context
As defined in the architecture (`docs/02-architecture.md`) and overview (`docs/01-overview.md`), the application is built using Tauri, Svelte, Vite, and Rust. It requires a clean foundation with Vanilla CSS for styling.

## Steps
1. **Scaffold Tauri Application:**
   - Use `create-tauri-app` (or similar) to initialize the project.
   - Select **Svelte** as the frontend framework.
   - Select **Vite** as the bundler.
   - Select **TypeScript** for the frontend language.
   - Select **Rust** for the backend language.

2. **Configure Frontend:**
   - Clean up boilerplate Svelte components.
   - Set up a base `app.css` for global Vanilla CSS styles (variables for colors, consistent spacing, reset).

3. **Configure Backend:**
   - Ensure the Rust environment (`Cargo.toml`) is correctly set up for Tauri.
   - Verify that the basic Tauri app builds and runs locally (`npm run tauri dev` or `cargo tauri dev`).

4. **Directory Structure:**
   - Establish standard directories: `src/components`, `src/stores`, `src/types` on the frontend, and structure the Rust `src-tauri/src` for future modules (e.g., `commands`, `models`).

## Completion Criteria
- A "Hello World" Tauri app runs successfully, displaying a basic Svelte page.
- Project structure is clean and ready for development.
