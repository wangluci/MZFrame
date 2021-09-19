/*
Navicat MySQL Data Transfer

Source Server         : helloword
Source Server Version : 50713
Source Host           : localhost:3306
Source Database       : test

Target Server Type    : MYSQL
Target Server Version : 50713
File Encoding         : 65001

Date: 2021-09-19 10:34:56
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
-- Table structure for mz_role_right
-- ----------------------------
DROP TABLE IF EXISTS `mz_role_right`;
CREATE TABLE `mz_role_right` (
  `RoleRightID` bigint(20) NOT NULL AUTO_INCREMENT,
  `RoleID` bigint(20) DEFAULT NULL,
  `RightCode` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`RoleRightID`)
) ENGINE=InnoDB AUTO_INCREMENT=895 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mz_user_right
-- ----------------------------
DROP TABLE IF EXISTS `mz_user_right`;
CREATE TABLE `mz_user_right` (
  `UserRightID` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) DEFAULT NULL,
  `RightCode` varchar(255) DEFAULT NULL,
  `RightType` int(11) DEFAULT NULL COMMENT '0：权限允许，1：禁止权限',
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
