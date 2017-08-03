<%@ Page Language="C#" %>
<% throw new Exception("Test exception"); %>
<!DOCTYPE html>
<%-- This is a means by which to test a server error that occurs outside the MVC pipeline --%>
