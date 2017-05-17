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



Throw Exception
===============

GET throwexception

Throws an exception



Get the request payload for a given Response-Id
===============================================

GET _request-payload/{id}

Gets the body of the request that corresponds to a given response Id

id - the id (obtained from the header of a response) to query against



Get requests for a given URI
============================

GET query/{method}?uri={uri}

Returns a list of all requests that have been made for a given URI, along with the full request query string (inluding path and parameters) and the ID of the associated response.

method - the HTTP method to configure (current supported values are GET or POST)
uri - the URI to configure (e.g. "group/search")



Check if a given URI was called with specific parameters
========================================================

GET assert/{method}?uri={uri-with-parameters}
GET assert/{method}?uri={uri-with-parameters}&times={times}

Checks if the specified uri (including parameters) was called (optionally checks if it was called a given number of times). Parameters are important when specifying the uri in this method
since it will only match calls that match your query *exactly*

method - the HTTP method to configure (current supported values are GET or POST)
uri-with-parameters - the URI to configure (e.g. "group/search?filter=foo&orderby=bar")