# PPWCode.vNext

This repository tracks the work on the next version of the major .NET PPWCode
libraries. The libraries are currently developped in the context of a single
Visual Studio solution and a single git repository to enable drastic
refactorings across the different libraries.

The code can be used directly from Git by including it in your own project using
a git submodule. No NuGet packages are released at this point, not even
prerelease packages. the Git submodule approach is explained further down this
document. Use is at your own risk: be aware that no apis offered by the
libraries should be considered stable at this point.


## Contributors

See the [GitHub Contributors list].


## ppwcode

This package is part of the ppwcode project by [PeopleWare n.v.].

More information can be found at [GitHub]. Check the different "net-ppwcode-*"
projects in particular for the .NET ppwcode projects.


## License and Copyright

Copyright 2014 - 2025 by [PeopleWare n.v.].

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.


## Use PPWCode.vNext

The following describes how you can currently use `ppwcode-vnext` in your own
solution. This is done by adding the `ppwcode-vnext` git repo as a submodule
inside your own code. This is a very simple approach and has a lot of benefits
at the current stage of development.

### create git submodule

We recommend the following commands to add the submodule inside your code base.

``` shell
# first go to the root of your code base
# then add the submodule in the folder `ppwcode-vnext`
#
# add `net-ppwcode-vnext` repo as submodule in folder `ppwcode-vnext` 
git submodule add https://github.com/peopleware/net-ppwcode-vnext.git ppwcode-vnext

# prev cmd uses the default branch of the `ppwcode-vnext` repo
# it is recommended to fix the submodule to a specific tag
# this can be done in the folder of the submodule, it is a git repo
cd ppwcode-vnext
git checkout preview/20250309b

# submodule is correctly configured now
# commit the submodule configuration
cd ..
git commit -m "Add submodule for ppwcode-vnext on preview/20250309b"
git push origin
```

Once these steps are done, the git submodule becomes available to everyone
working on the repo. A user who pulls this update locally, needs to execute the
following steps.

``` shell
# first go to the root of your code base
# then pull the changes from origin
git pull origin --ff-only

# local code base now contains submodule configuration
# however: submodule must still be initialized
#
# create the submodule folder and clone the repo
git submodule init

# switch to the registered version of the submodule code
# note: this switches to the tag preview/20250309b
git submodule update
```

Whenever pulled commits in your code base contain changes to the submodules, you
need to update the submodule to make it point to the correct tag/commit. This is
done as follows.

``` shell
git submodule update
```

To switch the submodule to another tag, the following commands need to be
executed.

``` shell
cd ppwcode-vnext
git checkout preview/20250309f
cd ..
git commit -m "Switch ppwcode-vnext to preview/20250309f"
```



[GitHub]: https://github.com/peopleware

[PeopleWare n.v.]: http://www.peopleware.be/

[GitHub]: https://github.com/peopleware

[GitHub Contributors list]: https://github.com/peopleware/net-ppwcode-vnext/graphs/contributors
