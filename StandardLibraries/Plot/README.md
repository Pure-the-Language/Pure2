﻿# (Experimental) Plot Library

Status: Usable, Expect Changes

Provides an experimental single-entry straightforward easy-to-use and remember 1D/2D plotting functionalities for numerical and catagorical data: `Plot(XAxis, Optional AXisSeries, Type, Settings)`. Alternatively, one could also use explicitly named functions for specific plot types.
This component is only responsible for gathering data and saving as pictures.
This is NOT part of Pure standard and may be removed without further notice.

Variations:

* `Plot(X, Y, Type, Settings)`
* `Plot(X, Y1-Y20, Type, Settings)`

General configurations:

* `Title`
* `XAxis`
* `YAxis`

Available plot types:

* Signal: affects `SignalSampleRate` option.

Technical note:

* Namespace is Graphing because one of the main method is called `Plot`; If we keep both there will be naming conflict that require fully denoting the names which is inconvinient to use in Pure.