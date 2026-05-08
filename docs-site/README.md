# docs-site

This folder contains the [Nanoc](https://nanoc.ws/) site used to turn the markdown files in `../documentation` into a static documentation site.

Nanoc is a Ruby static site generator. In this repo it reads markdown from `documentation/`, applies the layouts and filters in `docs-site/`, and writes the generated site to `docs-site/output/`.

The home page also includes an automatically generated site index built from the markdown files under `documentation/`.

When linking between source documents under `documentation/`, use relative `.md` links so the links work in GitHub's source view. The Nanoc markdown filter rewrites those links to the generated `index.html` routes during site compilation.

## Install dependencies

Run these commands from `docs-site/`, not from the repository root.

```powershell
cd docs-site
```

Then run:

```powershell
bundle install
```

The lockfile includes gem checksums. This is usually a one-off step once the `CHECKSUMS` section exists, but if `Gemfile.lock` changes because dependencies were added or updated, run:

```powershell
bundle lock --add-checksums
```

If `bundle` is not available, install Bundler first:

```powershell
gem install bundler
```

## Compile the docs

After manually editing any markdown file under `documentation/`, recompile the site from `docs-site/`:

```powershell
cd docs-site
bundle exec nanoc compile
```

This rebuilds the generated HTML in `docs-site/output/`.

GitHub Pages downloads Mermaid during the build and packages it into `output/assets/vendor/mermaid/`. If you want Mermaid diagrams to render when previewing `docs-site/output/` locally, download the same pinned asset after compiling:

```powershell
New-Item -ItemType Directory -Force output/assets/vendor/mermaid | Out-Null
Invoke-WebRequest -UseBasicParsing https://cdn.jsdelivr.net/npm/mermaid@11.14.0/dist/mermaid.min.js -OutFile output/assets/vendor/mermaid/mermaid-11.14.0.min.js
```

To check the result locally, open `docs-site/output/index.html` in a browser.

If a layout or filter change does not appear in the output, delete `docs-site/output/` and `docs-site/tmp/`, then run `bundle exec nanoc compile` again.

## Publish with GitHub Pages

This repository includes a GitHub Actions workflow at `.github/workflows/publish-docs-site.yml` that builds `docs-site/output/` and deploys it to GitHub Pages.

The published site is intended to be served from:

`https://dfe-digital.github.io/get-information-about-schools/`

The following was configured to host this nanoc site using Github pages

1. In the repository on GitHub, go to `Settings` -> `Pages`.
2. Under `Build and deployment`, set `Source` to `GitHub Actions`.
3. In `Settings` -> `Environments` -> `github-pages`, make sure the deployment branch rules allow the branch you want to publish from.
4. Push to the repository default branch, or run the workflow manually from the `Actions` tab.


