# Contributing to OneGame WorldBuilder

As is the nature of open-source projects, anyone can contribute to the OneGame WorldBuilder project. This document provides a simple guideline on how to provide contributions to the repository.


## Workflow

The OneGame project follows a variation of the [GitFlow model](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow), where `master` is a stable release branch and `develop` is the active working branch.  


To begin adding features, patches, etc.  Perform the following:

* Fork the repository
* Create a new branch with a name of the intent
    * Branch from `develop` for new features or improvements e.g. scripting features
    * Branch from `master` for critical hotfixes e.g. security flaws
* Commit your changes
* Create a pull request

### Creating Pull Requests

Your pull requests should have a short description detailing the changes you have made in your commits. Additionally, your branch should be rebased with the last commits to ensure your changes will not improperly conflict with the project.

## Copyright 
All work you submit will be under the MIT License. Any work that you contributed where you are not the original author must be credited with the original author's name and source, based on the type of work:

| Type | Examples | Action |
| -- | -- | -- | 
| Editable Text | Code | Add a comment header on the top of the file |
| Binary Assets | Textures, Models, Sound | Add a `Credits.md` file to the root location of the assets |