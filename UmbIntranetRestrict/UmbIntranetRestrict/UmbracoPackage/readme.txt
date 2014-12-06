Add a true/false property to the pages you want for your Intranet:
* umbIntranetRestrict

Insert the following appSettings keys in web.config:
* IntranetRestrict:IpAddress - IP in range to allow
* IntranetRestrict:SubnetMask - Subnet mask to use with specified IP address
* IntranetRestrict:UnauthorizedPageId - Page Id for people not in allowed range