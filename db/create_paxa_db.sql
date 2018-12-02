-- --------------------------------------------------------
-- Värd:                         127.0.0.1
-- Server version:               5.7.18-log - MySQL Community Server (GPL)
-- Server OS:                    Win64
-- HeidiSQL Version:             9.4.0.5125
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Dumping database structure for paxa
CREATE DATABASE IF NOT EXISTS `paxa` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `paxa`;

-- Dumping structure for tabell paxa.bookings
CREATE TABLE IF NOT EXISTS `bookings` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `resource_id` int(11) NOT NULL,
  `user_id` bigint(20) NOT NULL,
  `startTime` datetime NOT NULL,
  `endTime` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `resFK` (`resource_id`),
  KEY `userFK` (`user_id`),
  CONSTRAINT `resFK` FOREIGN KEY (`resource_id`) REFERENCES `resources` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `userFK` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8;

-- Dumpar data för tabell paxa.bookings: ~0 rows (approximately)
/*!40000 ALTER TABLE `bookings` DISABLE KEYS */;
/*!40000 ALTER TABLE `bookings` ENABLE KEYS */;

-- Dumping structure for tabell paxa.resourcecategory
CREATE TABLE IF NOT EXISTS `resourcecategory` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` tinytext NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;

-- Dumpar data för tabell paxa.resourcecategory: ~5 rows (approximately)
/*!40000 ALTER TABLE `resourcecategory` DISABLE KEYS */;
INSERT IGNORE INTO `resourcecategory` (`id`, `name`) VALUES
	(1, 'BCD'),
	(2, 'Båtar'),
	(3, 'Flaskor'),
	(4, 'Regulatorer'),
	(5, 'Övrigt');
/*!40000 ALTER TABLE `resourcecategory` ENABLE KEYS */;

-- Dumping structure for tabell paxa.resources
CREATE TABLE IF NOT EXISTS `resources` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` tinytext NOT NULL,
  `category` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_resources_resourcecategory` (`category`),
  CONSTRAINT `FK_resources_resourcecategory` FOREIGN KEY (`category`) REFERENCES `resourcecategory` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=29 DEFAULT CHARSET=utf8;

-- Dumpar data för tabell paxa.resources: ~28 rows (approximately)
/*!40000 ALTER TABLE `resources` DISABLE KEYS */;
INSERT IGNORE INTO `resources` (`id`, `name`, `category`) VALUES
	(1, 'Brum-Brum', 2),
	(2, 'Svarten', 2),
	(3, 'Oxybox', 5),
	(4, 'Regulatorpaket1', 4),
	(5, 'Regulatorpaket2', 4),
	(6, 'Regulatorpaket3', 4),
	(7, 'Regulatorpaket4', 4),
	(8, 'Regulatorpaket5', 4),
	(9, 'Oxybox cylinder', 5),
	(10, 'Väst 1 Balance(L)', 1),
	(11, 'Väst 2 Balance(L)', 1),
	(12, 'Väst 3 Balance(XL)', 1),
	(13, 'Väst 4 Balance(XL)', 1),
	(14, 'Väst 5 Balance(XL)', 1),
	(15, 'Väst 6 Scubapro(XS)', 1),
	(16, 'Väst 7 Scubapro(XS)', 1),
	(17, 'Väst 8 Scubapro(ML)', 1),
	(18, 'Väst 9 Scubapro(ML)', 1),
	(19, 'Väst 10 Poseidon (M)', 1),
	(20, 'Moby Dick', 2),
	(21, 'Flaska 1 (10x300)', 3),
	(22, 'Flaska 2 (10x300)', 3),
	(23, 'Flaska 3 (10x300)', 3),
	(24, 'Flaska 4 (10x300)', 3),
	(25, 'Flaska 5 (10x300)', 3),
	(26, 'Flaska 6 (10x300)', 3),
	(27, 'Flaska 7 (10x300)', 3),
	(28, 'Flaska 8 (8x300)', 3);
/*!40000 ALTER TABLE `resources` ENABLE KEYS */;

-- Dumping structure for tabell paxa.users
CREATE TABLE IF NOT EXISTS `users` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `profileid` varchar(255) NOT NULL,
  `name` tinytext NOT NULL,
  `email` tinytext,
  PRIMARY KEY (`id`),
  UNIQUE KEY `profileid` (`profileid`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8;

/*!40000 ALTER TABLE `users` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
