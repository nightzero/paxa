-- --------------------------------------------------------
-- Värd:                         127.0.0.1
-- Server version:               5.7.17-log - MySQL Community Server (GPL)
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
  `startTime` datetime NOT NULL,
  `endTime` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `resFK` (`resource_id`),
  CONSTRAINT `resFK` FOREIGN KEY (`resource_id`) REFERENCES `resources` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumpar data för tabell paxa.bookings: ~0 rows (approximately)
/*!40000 ALTER TABLE `bookings` DISABLE KEYS */;
/*!40000 ALTER TABLE `bookings` ENABLE KEYS */;

-- Dumping structure for tabell paxa.resources
CREATE TABLE IF NOT EXISTS `resources` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` tinytext,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;

-- Dumpar data för tabell paxa.resources: ~8 rows (approximately)
/*!40000 ALTER TABLE `resources` DISABLE KEYS */;
INSERT IGNORE INTO `resources` (`id`, `name`) VALUES
	(1, 'Brum-Brum'),
	(2, 'Svarten'),
	(3, 'Oxybox'),
	(4, 'Regulatorpaket1'),
	(5, 'Regulatorpaket2'),
	(6, 'Regulatorpaket3'),
	(7, 'Regulatorpaket4'),
	(8, 'Regulatorpaket5');
/*!40000 ALTER TABLE `resources` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;