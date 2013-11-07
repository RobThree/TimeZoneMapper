<img src="http://riii.nl/tzmapperlogo" width="32" height="32" alt="TimeZoneMapper Logo"> TimeZoneMapper
==============

Library for mapping \*N\*X TimeZone ID's (e.g. `Europe/Amsterdam`) to .Net's [TimeZoneInfo](http://msdn.microsoft.com/en-us/library/system.timezoneinfo.aspx) classes. This mapping is one-way since, for example, `Europe/Amsterdam` maps to `W. Europe Standard Time` but `W. Europe Standard Time` could map to `Europe/Stockholm` or `Arctic/Longyearbyen` just as easily.

The library provides a simple static `TimeZoneMap` object that exposes 3 types of mappers, each described below under [usage](#usage). The project is kept up-to-date with the latest mapping information as much as I can, but TimeZoneMapper can use the latest mapping information available online completely transparantly.

TimeZoneMapper is available as a [NuGet package](https://www.nuget.org/packages/TimeZoneMapper/) and comes with (basic) documentation in the form of a Windows Helpfile (.chm).

# Usage

The most basic example is as follows:
```c#
    TimeZoneInfo tzi = TimeZoneMap.DefaultValuesTZMapper.MapTZID("Europe/Amsterdam");
````

This uses the static TimeZoneMap.DefaultValuesTZMapper to map the string to the specific [TimeZoneInfo](http://msdn.microsoft.com/en-us/library/system.timezoneinfo.aspx). The DefaultValuesTZMapper object uses a built-in resource containing the mapping information. If you want more up-to-date mapping information, you can use the `OnlineValuesTZMapper`.
```c#
    TimeZoneInfo tzi = TimeZoneMap.OnlineValuesTZMapper.MapTZID("Europe/Amsterdam");
````

This will retrieve the [information](http://www.unicode.org/cldr/charts/latest/supplemental/zone_tzid.html) from the [Unicode Consortium](http://unicode.org/)'s latest [CLDR data](http://unicode.org/repos/cldr/trunk/common/supplemental/windowsZones.xml). There is a catch though: what if, for some reason, this information is not available (for example an outbound HTTP request is blocked, the data is not available (HTTP status 404 for example) or the data is corrupt (invalid XML for some reason))? Well, simple, we just use the `OnlineWithFallbackValuesTZMapper`!
```c#
    TimeZoneInfo tzi = TimeZoneMap.OnlineWithFallbackValuesTZMapper.MapTZID("Europe/Amsterdam");
````

This will try to download the CLDR data from the Unicode Consortium and when that, for some reason fails, it uses the built-in values as fallback.

Note that an HTTP request will be made only once for as long as the TimeZoneMapper is around (usually the lifetime of the application). Also note that the TimeZoneMapper is **case-*in*sensitive**; the TimeZone ID `Europe/Amsterdam` works just as well as `EUROPE/AMSTERDAM` or `eUrOpE/AmStErDaM`.

Finally, when you want control over the actual CLDR data and where it is stored, how you cache it etc. you can use the `CustomValuesTZMapper`. Be sure to add the `TimeZoneMapper.TZMappers` namespace if you want to use this class. This class' constructor has 3 overloads demonstrated below:

```c#
    // Overload 1: CustomValuesTZMapper(string, Encoding)
    
    // Load XML from file
    var mapper = new CustomValuesTZMapper("myfile.xml", Encoding.UTF8);
    TimeZoneInfo tzi = mapper.MapTZID("Europe/Amsterdam");
````    
```c#
    // Overload 2: CustomValuesTZMapper(string)
    
    // Get XML from database, cache, online resource, file, etc. or, in this case, "hard-coded":
    string cldrdata = "<supplementalData><windowsZones><mapTimezones otherVersion=\"xyz\" typeVersion=\"zyx\">..."; 
    var mapper = new CustomValuesTZMapper(cldrdata);
    TimeZoneInfo tzi = mapper.MapTZID("Europe/Amsterdam");
````
```c#
    // Overload 3: CustomValuesTZMapper(Stream)
    
    // Use a Stream
    using (var mystream = new GZipStream(File.OpenRead("myfile.gz"), CompressionMode.Decompress))
    {
        var mapper = new CustomValuesTZMapper(mystream);
        TimeZoneInfo tzi = mapper.MapTZID("Europe/Amsterdam");
    }
````
All you need to do is ensure the data you supply to the CustomValuesTZMapper is valid CLDR data (see [this example](TimeZoneMapper/ResourceFiles/windowsZones.xml))
# Future

I will try to update the built-in resource every now-and-then.
