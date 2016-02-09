# UmbIntranetRestrict
Event handler plugin for restricting parts of an Umbraco website to an Intranet.

## What's inside

This project includes a DLL that will register as an event handler with Umbraco. If pages are specified with the proper properties, it will restrict access to those pages to specified networks.

## System requirements
1. NET Framework 4.5
2. Umbraco 7.3.7+ (should work with older versions but not tested)

## NuGet availability
This project is available on [NuGet](https://www.nuget.org/packages/UmbIntranetRestrict/).

## Usage instructions
### Getting started
1. Add **UmbIntranetRestrict.dll** as a reference in your project or place it in the **\bin** folder.
2. Add the dependency [**IPNetwork2**](https://github.com/lduchosal/ipnetwork) as a reference in your project or place its DLLs in the **\bin** folder.
3. Insert the following `<appSettings>` keys in **web.config**:
  - `IntranetRestrict:IpAddress` - comma-separated list of IP addresses (IPv4 or IPv6) in range to allow
  - `IntranetRestrict:SubnetMask` - comma-separate list of subnet masks (IPv4 or IPv6) to use with specified IP address
  - `IntranetRestrict:IpNetwork` - comma separated list of IP networks (IPv4 or IPv6) to allow (e.g., `127.0.0.1/32` to allow just localhost; can be used in place of `IntranetRestrict:IpAddress` and `IntranetRestrict:SubnetMask`
  - `IntranetRestrict:UnauthorizedPageId` - Umbraco page ID to display for people whose computers are not in allowed range
4. To restrict access to specific pages, add a true/false-type document property in Umbraco:
  - `umbIntranetRestrict`
