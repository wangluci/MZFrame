# MZFrame

#### 介绍
兼容.net core和.net framework的mvc框架，并可让.net framework实现嵌入式http服务器，实现模块化热更新，超简单的orm框架。

#### 软件架构
软件架构说明

MainWeb目录为.net framework下mvc的测试项目

MyAccess目录包含orm框架、json、日志、分词等

MyNet目录为异步的、事件驱动的网络应用程序框架

MyNet.TemplateAction目录.net framework嵌入式http服务器，并提供mvc框架支持

ResumeML目录为.net core的mvc实现例子，是一个简历识别的例子，可以用来识别简历的字段

SysWeb.TemplateAction目录为mvc框架asp.net的支持库

TemplateAction目录为mvc框架

TemplateAction.NetCore目录为mvc框架.net core支持库

TemplateAction.Route目录为mvc框架的路由模块

TestService目录为mainweb测试项目的一个模块



#### 安装教程
需要至少vs2019打开,
MZFrame.sln项目为.net framework使用示例
ResumeML/ResumeML.sln项目为.net core使用示例

使用方法可查看ResumeML目录和MainWeb目录里的例子

#### 使用说明

1.  MVC视图语法说明
    视图代码应该用@{代码}或直接@代码然后换行的方式，
    不推荐用标签<loop></loop>的写法，只有在<m:def var="变量名"></m:def>才推荐
    - 变量定义
    变量有两种写法$a和a
    
    - 循环语句for
    for语句有两种写法for(($a,$i) in $b)和for($a in $b)
    其中$a变量为集合$b的元素，$i变量为索引号,$b为集合
    
    
2.  xxxx
3.  xxxx

#### 参与贡献

1.  Fork 本仓库
2.  新建 Feat_xxx 分支
3.  提交代码
4.  新建 Pull Request


#### 特技

1.  使用 Readme\_XXX.md 来支持不同的语言，例如 Readme\_en.md, Readme\_zh.md
2.  Gitee 官方博客 [blog.gitee.com](https://blog.gitee.com)
3.  你可以 [https://gitee.com/explore](https://gitee.com/explore) 这个地址来了解 Gitee 上的优秀开源项目
4.  [GVP](https://gitee.com/gvp) 全称是 Gitee 最有价值开源项目，是综合评定出的优秀开源项目
5.  Gitee 官方提供的使用手册 [https://gitee.com/help](https://gitee.com/help)
6.  Gitee 封面人物是一档用来展示 Gitee 会员风采的栏目 [https://gitee.com/gitee-stars/](https://gitee.com/gitee-stars/)
