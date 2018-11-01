# Saved test screenshots
This folder stores the reference screenshots for all screenshot tests.

## Single reference image
A single reference image is stored in the following format:

> <browser>.<type>.<name>.png

`browser` and `type` are optional. 

The following values are valid for `browser`:
 - _Chrome_
 - _InternetExplorer_ 

The following values are valid for `type`:
 - _any_ (matches any of the following)
 - _Desktop_
 - _Browser_

Examples:

```
Chrome.any.Test.png
InternetExplorer.Browser.Test.png
Desktop.Test.png
Test.png
```

## Multiple reference images
In order to store multiple images for one test the following format should be used:

> <browser>.<type>.<name><id>.png

Rules for `browser` and `type` are the same as above.

`id` is a sequential number starting at `0`

Examples:

```
Desktop.Test0.png
Desktop.Test1.png
Desktop.Test2.png
```
## Transparency

In order to ignore certain part of the screenshot comparison the color can be set to transparent in the source file. This skips the equality check for that pixel. This is useful in cases where browsers could render content differently, but the content itself is not important for the test.

## Leniency

When comparing screenshot images there can also be a certain part of leniency configured. This is very useful as some colors are rendered with small differences on different PCs. There are two values to configure:

- _MaxAllowedPixelVariance:_ The maximum amount of difference in RGBA (additive) before a pixel gets flagged as different.
- _UnequalPixelThreshold:_ A percent value indicating how many "flagged" pixels are allowed before deeming the images to be not equal.