# Important Note
This repository is forked from the `dnut/Rewrap` repository `master` branch.

When working with Jinja templates, it is traditional to append a `.j2` file extension to
whatever file type you are working with.  For example, and html file becomes `.html.j2`.
Most editors recognise this and, in this example, will then correctly syntax highlight
for both html and jinja variables/macros.  However...rewrap does not recognise this and
will refuse to operate on any file with the `.j2` extension. Rather than break convention,
and potentially break syntax highlighting, I have forked and hacked *Rewrap Revived* to
recognise the `.j2` , `.jinja`, and `.jinja2` extensions for all file types currently
supported.

I have imlemented this by modifying the `Language` module in `/core/Parsking.Language.fs`.
I have not raised a PR as this is an absolutely colossal hack to solve my immediate problem.

There are a few major things missing from my fork:
- Planning and foresight.
- A good understand of the code, especially ID versus file extension.
- Any form of comprehensive tests.
- Syntax support jinja inside those overriden files.


### Build & Install
The original repository has a github worker to build the extension. If you have docker,
you can avoid the need to install node/nvm/dotnet and so on as below:

Note that the versions here are mostly set to match the github worker except that we
are using the oldest node LTS available (18) as 14 is no longer supported and a PITA
to install.  We are using version 6.0 of the .NET SDK otherwise Fable hangs.  Finally
we are using TERM_PROGRAM=vscode to avoid running tests which require a bunch more
packages to be installed.

```
docker run --rm -it --platform linux/amd64 \
  -e DEBIAN_FRONTEND=noninteractive \
  -e TZ=Etc/UTC \
  -e TERM_PROGRAM=vscode \
  -v "$PWD":/src:ro \
  -v "$PWD":/out \
  -w /work \
  ubuntu:24.04 bash -lc '
    set -e
    apt-get update
    apt-get install -y curl git ca-certificates gnupg build-essential wget tzdata libicu-dev
    mkdir -p /work
    tar -C /src -cf - . | tar -C /work -xf -
    cd /work
    curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
    bash dotnet-install.sh --channel 6.0 --install-dir /opt/dotnet-6
    export PATH=/opt/dotnet-6:$PATH
    export DOTNET_ROOT=/opt/dotnet-6
    ver=$(dotnet --list-sdks | awk "/^6\\./{print \$1; exit}")
    printf "{ \"sdk\": { \"version\": \"%s\" } }\n" "$ver" > global.json
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash -
    apt-get install -y nodejs
    npm install -g npm
    if [ -f package-lock.json ]; then npm ci; elif [ -f package.json ]; then npm install; fi
    rm -rf .obj/vscode-test
    chmod +x ./do || true
    ./do package -v
    cp .obj/Rewrap-VSCode.vsix /out/Rewrap-VSCode.vsix
  '
```

This can then be installed by accessing the VS Code Command Palette using either `Ctrl + Shift + P`
or `Cmd + Shift + P` (as appropriate to your environment) and then selecting: `Extensions: Install from VSIX...`.

The vsix package produced is architecture independent.


<!-- This part has to be written in HTML, because doing it in markdown puts the content in
a <p>, which adds unwanted margins. It has to be in a table so it can be right-aligned on
GitHub. For GitHub we can't get rid of the border on the td nor make the font smaller as
we want-->
<table class="topright" align="right" style="font-size:90%;width:auto;margin:0;border:none">
<tr style="border:none"><td align="right" style="border:none">
For <a href="https://marketplace.visualstudio.com/items?itemName=dnut.rewrap-revived"><b>VS Code</b></a>,
<a href="https://open-vsx.org/extension/dnut/rewrap-revived"><b>Open VSX</b></a> and
<a href="https://marketplace.visualstudio.com/items?itemName=stkb.Rewrap-18980">
  <b>Visual Studio</b></a>.<br/>
Latest stable version <b>1.16.3</b> / pre-release <b>17.x</b> /
<a href="https://github.com/dnut/rewrap/releases">changelog</a>
</td></tr></table>


<h1 style="font-size: 2.5em">Rewrap Revived</h1>

Rewrap Revived is a Visual Studio and VS Code extension that is used to hard-wrap code 
comments to a configured maximium line length. This is a fork of the unmaintained 
[Rewrap](https://github.com/stkb/Rewrap) extension by Steve Baker 
([@stkb](https://github.com/stkb)).

<br><img src="https://dnut.github.io/Rewrap/images/example.svg" width="700px"/><br/><br/>

The main Rewrap command is: <sn>**Rewrap Comment / Text**</sn>, by default bound to
`Alt+Q`. With the cursor in a comment block, hit this to re-wrap the contents to the
[specified wrapping column](https://dnut.github.io/Rewrap/configuration/#wrapping-column).

## Features

* Re-wrap comment blocks in many languages, with per-language settings.
* Smart handling of contents, including Java-/JS-/XMLDoc tags and code examples.
* Can select lines to wrap or multiple comments/paragraphs at once (even the whole
  document).
* Also works with Markdown documents, LaTeX or any kind of plain text file.

The contents of comments are usually parsed as markdown, so you can use lists, code
samples (which are untouched) etc:

<img src="https://dnut.github.io/Rewrap/images/example1.svg" width="700px"/>

<div class="hideOnDocsSite"><br/><b><a href="https://dnut.github.io/Rewrap/">
See the docs site for more info.</a></b></div>

## Installation

Rewrap Revived is available in both the
[Microsoft marketplace](https://marketplace.visualstudio.com/items?itemName=dnut.rewrap-revived)
and the [OpenVSX marketplace](https://open-vsx.org/extension/dnut/rewrap-revived).

**Please install the pre-release version**. That way, you can identify any bugs and report
them, so they don't make their way into the stable release. If you *do* observe a bug, then you
can switch to the stable release, and rest assured that the bug will not be introduced there,
since you have reported the issue (unless of course, it is already present in both releases).

