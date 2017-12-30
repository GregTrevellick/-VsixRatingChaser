[AppVeyorProjectUrl]: https://ci.appveyor.com/project/GregTrevellick/vsixratingchaser
[AppVeyorProjectBuildStatusBadgeSvg]: https://ci.appveyor.com/api/projects/status/5ism52msmffomkh3?svg=true
[GitHubRepoURL]: https://github.com/GregTrevellick/VsixRatingChaser
[GitHubRepoIssuesURL]: https://github.com/GregTrevellick/VsixRatingChaser/issues
[GitHubRepoPullRequestsURL]: https://github.com/GregTrevellick/VsixRatingChaser/pulls
[NugetUrl]: https://www.nuget.org/packages/OpenInApp.Common/
[VisualStudioURL]: https://www.visualstudio.com/
[CharityWareURL]: https://github.com/GregTrevellick/MiscellaneousArtefacts/wiki/Charity-Ware
[WhyURL]: https://github.com/GregTrevellick/MiscellaneousArtefacts/wiki/Why

# Vsix Rating Chaser 

[![Licence](https://img.shields.io/aur/license/yaourt.svg)](/LICENSE.txt)
[![Build status][AppVeyorProjectBuildStatusBadgeSvg]][AppVeyorProjectUrl]
[![CharityWare](https://img.shields.io/badge/Charity%20Ware-Thank%20You-brightgreen.svg)][CharityWareURL]

![Vsix Rating Chaser](NugetIcon_64x64.png "Vsix Rating Chaser Logo")

A package to gently encourage reviews for [Visual Studio][VisualStudioURL] extensions.

Available for download at the [nuget gallery][NugetUrl].

## Introduction

Ever noticed how few people ever bother to rate a visual studio extension, even ones that are completely free and are feature-rich ?

For example, [Developer Analytics Tools](https://marketplace.visualstudio.com/items?itemName=VisualStudioOnlineApplicationInsights.DeveloperAnalyticsTools) has over 140m installs but only 7 reviews.

[GitHub Extension for Visual Studio](https://marketplace.visualstudio.com/items?itemName=GitHub.GitHubExtensionforVisualStudio) fairs slightly better at 57 reviews for 37m installs.

So I decided to build a package that visual studio extension authors could use to **gently** and **unobtrusively** encourage their users to review their extensions.

## How It Works

Your visual studio extension will call into this package, and it will, at **three month intervals**, for **a maximum of three times**, present the user with a pop-up window asking for a review.

It is deliberately low-key, and does not actually track if a review has been made.

It does not disable any functionality of your vsix if a review hasn't been made (how could it?).

The pop-up request explains how your extension was lovingly created for free by you, and how you don't get paid for it. 

I did consider making the text configurable, but in the interests of simplicity, and because this package is not aimed at money-making corporations (who, let's face it, can easily create their own equivalent package if they want to) I left the text hard-coded as per the screen shot below.

![Rating Request](\src\VsixRatingChaser\RatingRequestScreenshot.png)

## How To Use This Package

1. Add the [nuget package][NugetUrl] package to your vsix solution

1. Add a class to your vsix application gregt1 inherits from DialogPage, implements IRatingDetailsDto

1. Add a class to your vsix application gregt2 gets the options, calls the Chase() method in Chaser class

That's it. 

The package takes care of persisting the date that the next pop-up is due, and monitors how many pop-ups the user has already been presented with, so that they don't get prompted to review indefinitely.

The response from the call can be interrogated, but essentially explains:
 - If the pop-up was not displayed or not 
 - If the pop-up was displayed then also indicates if the user clicked the link
 - The reason the pop-up could not be displayed if any data was invalid (e.g. the caller did not supply a name for the vsix)

For example:

    using Microsoft.VisualStudio.Shell;
    using System;
    using VsixRatingChaser;
    using VsixRatingChaser.Dtos;
    using VsixRatingChaser.Enums;
    using VsixRatingChaser.Interfaces;
    
    namespace MyVsix
    {
        public class MyVsixRatingDetailsDto : DialogPage, IRatingDetailsDto
        {
            public DateTime PreviousRatingRequest { get; set; }
            public int RatingRequestCount { get; set; }
        }
    
        public class SampleCaller
        {
            public static ChaseOutcome MyVsixSampleCall()
            {
                var ratingDetailsDto = (IRatingDetailsDto)Microsoft.VisualStudio.Shell.GetDialogPage(typeof(MyVsixRatingDetailsDto));
    
                var extensionDetailsDto = new ExtensionDetailsDto
                {
                    AuthorName = "Greg Trevellick",
                    ExtensionName = "Open in Paint.NET",
                    MarketPlaceUrl = "https://marketplace.visualstudio.com/items?itemName=GregTrevellick.OpeninPaintNET"
                };
    
                var chaser = new Chaser();
    
                return chaser.Chase(ratingDetailsDto, extensionDetailsDto);
            }
        }
    }

### License

Software License is available [here](/LICENSE.txt).

### Miscellaneous

Please consider this a [charity ware][CharityWareURL] package.

Bugs can be logged [here][GitHubRepoIssuesURL].

See the [change log](CHANGELOG.md) for release history.

[Why build this package?][WhyURL]