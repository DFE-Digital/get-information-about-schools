Configure responses using the following method:

Configure Response
==================

PUT configure/{method}?uri={uri}

Configure the response for a given method on a given URI

method - the HTTP method to configure (current supported values are GET or POST)
uri - the URI to mock (e.g. "group/search")

Whatever data you send in the body will be replayed when the given URI is hit with the configured verb. It is important that you use the correct Content-Type header for the body since this will also be replayed

The following are examples of multi-part responses and Json responses but any response type should work.

Multi-part Messages
-------------------

Content-Type: multipart/mixed; boundary="simple boundary"

This is the preamble.  It is to be ignored, though it 
is a handy place for mail composers to include an 
explanatory note to non-MIME compliant readers. 
--simple boundary 

This is implicitly typed plain ASCII text. 
It does NOT end with a linebreak. 
--simple boundary 
Content-type: text/plain; charset=us-ascii 

This is explicitly typed plain ASCII text. 
It DOES end with a linebreak. 

--simple boundary-- 
This is the epilogue.  It is also to be ignored.


JSON
----

Content-Type: application/json

{"count":5683,"items":[{"groupUID":1520,"name":"*Co-op Brent Knoll and Watergate Co-operative Trust","companiesHouseNumber":null,"groupTypeId":2,"closedDate":null,"statusId...



Get Configured Responses
========================

GET configure

Returns a list of currently configured responses



Delete Response
===============

DELETE configure/{method}?uri={uri}

Deletes a given configuration

method - the HTTP method to configure (current supported values are GET or POST)
uri - the URI to configure (e.g. "group/search")



Delete All Responses
====================

DELETE configure

Deletes *all* configuration