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

## Muliple reference images
In order to store multiple images for one test the following format should be used:

> <browser>.<type>.<name><id>.png

Rules for `browser` and `type` are the same as above.

`id` is a sequencial number starting at `0`

Examples:

```
Desktop.Test0.png
Desktop.Test1.png
Desktop.Test2.png
```