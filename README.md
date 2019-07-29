An Example of .Net Core 2 Web Application With MVC 

In this Web Application;
1-	Role-based authorization is used.
2-	Logging Mechanism is used.
3-	Changes in the Tables are recorded by AutoHistory.
4-	When I select a Brand, Model List is changed via Ajax and Json.
5-	Show information message with Popup, when inserting, updating and deleting operations happen.
6-	Paging and Searching in List Pages.
7-	Search Dropdownlist is used.
8-	when an error occurs, Error redirection and showing error messages. 
9-	Logout process is shown.

I got the Application’s UI from “https://www.coderepo.blog/posts/building-elegant-applications-aspnet-core-mvc-2.1-coreui-2-bootstrap-4/”. Then I integrated it into my own application.
I used LazZiya.TagHelpers(2.2.1) Nuget Package for Paging
I used Microsoft.EntityFrameworkCore.AutoHistory Nuget Package for AutoHistory.
I used NetEscapades.Extensions.Logging.RollingFile Nuget Package for Logging.

There are 7 tables, which are Employee, User, Role, UserRole, Brand, Model and Product. When the application runs for the first time, some records are added. Admin’s Username is “admin” and password is “1”.
There are 3 Roles => Admin, Human Resource, Stock.
“admin” User has all 3 Roles.
We must define Employees before User definition.
Every save, update and delete events are saved by Logging Mechanism. And the results of these events are written by AutoHistory Mechanism.
Brand and Model are related. When Brand changes, Model changes too. We use Ajax and Json to load Model List.
We use Paging Mechanism all List Pages.
We use Jquery for Search Dropdownlist in Product’s Create.cshtml and Edit. cshtml.

I hope this little app can help you to understand the Role-based authorization, Logging, AutoHistory, Ajax with Json, Paging, Search Dropdonwlist and solve yours problems.

If you have questions, please write me.
