/*
Navicat MySQL Data Transfer

Source Server         : helloword
Source Server Version : 50713
Source Host           : localhost:3306
Source Database       : test

Target Server Type    : MYSQL
Target Server Version : 50713
File Encoding         : 65001

Date: 2021-09-24 12:13:51
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for mz_admin
-- ----------------------------
DROP TABLE IF EXISTS `mz_admin`;
CREATE TABLE `mz_admin` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(128) DEFAULT NULL,
  `RealName` varchar(20) DEFAULT NULL,
  `Password` varchar(64) DEFAULT NULL,
  `Avatar` varchar(255) DEFAULT NULL,
  `Introduction` varchar(500) DEFAULT NULL,
  `BindId` varchar(128) DEFAULT NULL,
  `CreatedOn` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `AdmUserName` (`UserName`),
  UNIQUE KEY `AdmBind` (`BindId`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mz_role
-- ----------------------------
DROP TABLE IF EXISTS `mz_role`;
CREATE TABLE `mz_role` (
  `RoleID` bigint(20) NOT NULL AUTO_INCREMENT,
  `RoleName` varchar(255) DEFAULT NULL,
  `RoleDesc` varchar(1000) DEFAULT NULL,
  `RoleType` int(11) DEFAULT '0',
  `CreateUserId` bigint(20) DEFAULT NULL,
  `CreateDate` datetime DEFAULT NULL,
  PRIMARY KEY (`RoleID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mz_role_permission
-- ----------------------------
DROP TABLE IF EXISTS `mz_role_permission`;
CREATE TABLE `mz_role_permission` (
  `RoleRightID` bigint(20) NOT NULL AUTO_INCREMENT,
  `RoleID` bigint(20) DEFAULT NULL,
  `RightCode` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`RoleRightID`)
) ENGINE=InnoDB AUTO_INCREMENT=895 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mz_sys_log
-- ----------------------------
DROP TABLE IF EXISTS `mz_sys_log`;
CREATE TABLE `mz_sys_log` (
  `SysLogID` bigint(11) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) DEFAULT NULL COMMENT '?????????',
  `CreateDate` datetime DEFAULT NULL,
  `LogType` smallint(6) DEFAULT NULL COMMENT '????????????:0?????????',
  `IPAddress` varchar(50) DEFAULT NULL COMMENT 'ip??????',
  `Info` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`SysLogID`),
  KEY `syslogloginname` (`UserId`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mz_user_permission
-- ----------------------------
DROP TABLE IF EXISTS `mz_user_permission`;
CREATE TABLE `mz_user_permission` (
  `UserRightID` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) DEFAULT NULL,
  `RightCode` varchar(255) DEFAULT NULL,
  `RightType` int(11) DEFAULT NULL COMMENT '0??????????????????1???????????????',
  PRIMARY KEY (`UserRightID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mz_user_role
-- ----------------------------
DROP TABLE IF EXISTS `mz_user_role`;
CREATE TABLE `mz_user_role` (
  `UserRoleID` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) DEFAULT NULL,
  `RoleID` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`UserRoleID`),
  KEY `UserRoleID` (`RoleID`) USING HASH,
  KEY `UserLoginName` (`UserId`) USING HASH
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for testtb
-- ----------------------------
DROP TABLE IF EXISTS `testtb`;
CREATE TABLE `testtb` (
  `testid` int(11) NOT NULL AUTO_INCREMENT,
  `testdes` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`testid`)
) ENGINE=InnoDB AUTO_INCREMENT=156 DEFAULT CHARSET=utf8mb4;
