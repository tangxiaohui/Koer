/* 
 LuaFramework Code By Jarjin lee 
*/
using System;

public interface ICommodMessage
{
	string Name { get; }

	object Body { get; set; }
		
	string Type { get; set; }

    string ToString();
}

