

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for testtb
-- ----------------------------
DROP TABLE IF EXISTS `testtb`;
CREATE TABLE `testtb` (
  `testid` int(11) NOT NULL AUTO_INCREMENT,
  `testdes` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`testid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
