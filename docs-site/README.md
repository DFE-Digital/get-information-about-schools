# docs-site

This folder contains the [Nanoc](https://nanoc.ws/) site used to turn the markdown files in `../documentation` into a static documentation site.

Nanoc is a Ruby static site generator. In this repo it reads markdown from `documentation/`, applies the layouts and filters in `docs-site/`, and writes the generated site to `docs-site/output/`.

The home page also includes an automatically generated site index built from the markdown files under `documentation/`.

## Install dependencies

Run these commands from `docs-site/`, not from the repository root.

```powershell
cd docs-site
```

Then run:

```powershell
bundle install
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

To check the result locally, open `docs-site/output/index.html` in a browser.

If a layout or filter change does not appear in the output, delete `docs-site/output/` and `docs-site/tmp/`, then run `bundle exec nanoc compile` again.
