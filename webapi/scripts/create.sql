CREATE TABLE company (
  Id INT(11) NOT NULL AUTO_INCREMENT,
  CompanyName VARCHAR(100) NOT NULL,
  Logotype BLOB DEFAULT NULL,
  Identifier VARCHAR(20) NOT NULL,
  CompanyAddress VARCHAR(200) NOT NULL,
  PhoneNumber VARCHAR(11) NOT NULL,
  PromotionAt INT(11) NOT NULL,
  PromotionDescription VARCHAR(500) NOT NULL,
  PromotionMessage VARCHAR(500) NOT NULL,
  ExpiresAt DATETIME DEFAULT NULL,
  PRIMARY KEY (Id)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

CREATE TABLE employee (
  CompanyId INT(11) NOT NULL,
  Id INT(11) NOT NULL,
  Name VARCHAR(100) NOT NULL,
  MobileNumber VARCHAR(11) NOT NULL,
  PRIMARY KEY (CompanyId, Id),
  FOREIGN KEY (CompanyId) REFERENCES company(Id)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

CREATE TABLE branch (
  CompanyId INT(11) NOT NULL,
  BranchId INT(11) NOT NULL,
  AccessCode VARCHAR(50) NOT NULL,
  Description VARCHAR(100) NOT NULL,
  Address VARCHAR(500) NOT NULL,
  PhoneNumber VARCHAR(11) NOT NULL,
  Active INT(1) NOT NULL,
  PRIMARY KEY (CompanyId, BranchId),
  FOREIGN KEY (CompanyId) REFERENCES company(Id)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

CREATE TABLE customer (
  Id INT(11) NOT NULL AUTO_INCREMENT,
  Name VARCHAR(100) NOT NULL,
  Identifier VARCHAR(20) NOT NULL,
  Address VARCHAR(200) NOT NULL,
  PhoneNumber VARCHAR(11) NOT NULL,
  MobileNumber VARCHAR(11) NOT NULL,
  Email VARCHAR(100) NOT NULL,
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

CREATE TABLE registry (
  DeviceId VARCHAR(50) NOT NULL,
  CompanyId INT(11) NOT NULL,
  CustomerId INT(11) NOT NULL,
  RegisterDate DATETIME NOT NULL,
  VisitCount INT(11) NOT NULL,
  Status VARCHAR(1) NOT NULL,
  PRIMARY KEY (DeviceId, CompanyId, CustomerId),
  FOREIGN KEY (CompanyId) REFERENCES company(Id),
  FOREIGN KEY (CustomerId) REFERENCES customer(Id)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

CREATE TABLE activity (
  Id INT(11) NOT NULL AUTO_INCREMENT,
  DeviceId VARCHAR(50) NOT NULL,
  CompanyId INT(11) NOT NULL,
  CustomerId INT(11) NOT NULL,
  BranchId INT(11) NOT NULL,
  EmployeeId INT(11) NOT NULL,
  VisitDate DATETIME NOT NULL,
  Applied INT(1) NOT NULL,
  PRIMARY KEY (Id),
  FOREIGN KEY (DeviceId, CompanyId, CustomerId) REFERENCES registry(DeviceId, CompanyId, CustomerId),
  FOREIGN KEY (CompanyId, BranchId) REFERENCES branch(CompanyId, BranchId)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

CREATE TABLE user (
  Id INT(11) NOT NULL AUTO_INCREMENT,
  Username VARCHAR(10) NOT NULL,
  Password VARCHAR(100) NOT NULL,
  Identifier VARCHAR(20) NULL,
  PRIMARY KEY (Id)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

CREATE TABLE role (
  Id INT(11) NOT NULL AUTO_INCREMENT,
  Description VARCHAR(500) NOT NULL,
  PRIMARY KEY (Id)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

CREATE TABLE roleperuser (
  UserId INT(11) NOT NULL,
  RoleId INT(11) NOT NULL,
  PRIMARY KEY (UserId, RoleId),
  FOREIGN KEY (UserId) REFERENCES user(Id),
  FOREIGN KEY (RoleId) REFERENCES role(Id)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

CREATE TABLE parameter (
  Id int(11) NOT NULL,
  Value VARCHAR(100) NOT NULL,
  PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE authorizationentry (
 Id varchar(36) NOT NULL,
 EmitedAt datetime NOT NULL,
 PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO parameter VALUES (1, 'UNfxAQwIEWg8kil6t2woauT7LoJPyJ66ARYibyW9');
INSERT INTO user values (1, 'ADMIN', '/SOHlDytfCDqqGitmLZJgw==');
INSERT INTO user values (2, 'MOBILEAPP', '/SOHlDytfCDqqGitmLZJgw==');
INSERT INTO role VALUES (1, 'ADMIN');
INSERT INTO role VALUES (2, 'Actualiza datos de la empresa');
INSERT INTO role VALUES (3, 'Mantenimiento de empleados');
INSERT INTO role VALUES (4, 'Mantenimiento de clientes');
INSERT INTO role VALUES (5, 'Menu de reportes');
INSERT INTO role VALUES (6, 'Registro de clientes');
INSERT INTO role VALUES (7, 'Registro de visitas');
INSERT INTO roleperuser VALUES (1,1);
INSERT INTO roleperuser VALUES (2,6);
INSERT INTO roleperuser VALUES (2,7);