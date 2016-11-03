# FF14_Blacklist

一个FF14的简易黑名单工具。

当前功能：
1. 添加黑名单（服务器 + ID + 黑名单理由）；
2. 根据服务器 or ID 查找是否有黑历史；

正在开发功能：
1. 黑历史详细备注，以及双击查询结果可查看这个备注；
2. 黑名单数据导入导出（数据共享）

简介：
这是一个简易黑名单工具[删除线]专为贴吧PVP服务[/删除线]。 由于游戏内嵌的黑名单系统无法跨服黑名单，也没有办法记录黑名单原因，比较不方便，所以做了一个简单的工具，来记录这些东西。

使用C# + SQLite开发，理论上有安装FF14的机器基本上可以直接使用（依赖.NET平台，FF14安装的时候应该已经安装）。 数据库备份的话暂时可以直接复制文件夹内的.db文件，以后会增加导入导出的功能。

===

A simple Blacklist Tool for FF14 Chinese players, especially for the players who log in Baidu Tieba usually.
For some reasons, the original FF14 blacklist system within the game client is not that useful for many users. Neither can it mark the players not in the same server, nor can it comment the blacklist reasons.
So I made a small software, a .NET winform software, no need other using environment, just the same as FF14 itself.

Please feel free to contact when you meet any problem while using.
