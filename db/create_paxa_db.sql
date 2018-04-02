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
  `user_id` bigint(20) NOT NULL,
  `startTime` datetime NOT NULL,
  `endTime` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `resFK` (`resource_id`),
  KEY `userFK` (`user_id`),
  CONSTRAINT `resFK` FOREIGN KEY (`resource_id`) REFERENCES `resources` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `userFK` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON UPDATE CASCADE
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
	(3, 'Oxybox kvadratisk'),
	(4, 'Regulatorpaket1'),
	(5, 'Regulatorpaket2'),
	(6, 'Regulatorpaket3'),
	(7, 'Regulatorpaket4'),
	(8, 'Regulatorpaket5'),
	(9, 'Oxybox cylinder'),
	(10, 'Väst 1 Balance(L)'),
	(11, 'Väst 2 Balance(L)'),
	(12, 'Väst 3 Balance(XL)'),
	(13, 'Väst 4 Balance(XL)'),
	(14, 'Väst 5 Balance(XL)'),
	(15, 'Väst 6 Scubapro(XS)'),
	(16, 'Väst 7 Scubapro(XS)'),
	(17, 'Väst 8 Scubapro(ML)'),
	(18, 'Väst 9 Scubapro(ML)'),
	(19, 'Väst 10 Poseidon (M)');
/*!40000 ALTER TABLE `resources` ENABLE KEYS */;

-- Dumping structure for tabell paxa.users
CREATE TABLE IF NOT EXISTS `users` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `profileid` varchar(255) NOT NULL,
  `name` tinytext NOT NULL,
  `email` tinytext,
  PRIMARY KEY (`id`),
  UNIQUE KEY `profileid` (`profileid`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- Dumpar data för tabell paxa.users: ~2 rows (approximately)
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
/*!40000 ALTER TABLE `users` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
