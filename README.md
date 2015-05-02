# PUC-ScheduleMaker

## Introduction

This project involves redesign of the PUC online class schedule application. The deliverable will be a desktop application for PUC class schedules, with easy to use and more functional features. In this report, the redesigned database for storing course information will be introduced in the following sections.

## Database Logical Design
  Based on the study of data provided by instructor, the unified schema (ERD) for class schedule is created, shown as the following: 
  ![alt text](https://github.com/EdwardTang/PUC-ScheduleMaker/blob/master/Data%20Model/ITS462_data_model.jpg)

# Data Dictionary

| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Subject  | A branch knowledge taught in the school. The table name of the entity is "Subjects". | subjectId | Primary key of a subject record in the table "Subjects". | INTEGER | True | True   | True |
| subject | Subject name in acronym, e.g. CGT, ITS. | TEXT | False | True | True |


| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Terms  | The time during which school holds classes. The table name of the entity is "Terms". | termId | Primary key of a term record in the table "Terms". | INTEGER | True | True | True |
|   |   | quarter | Quarter name in each term, e.g. Fall, Spring, Summer. | TEXT | False | True | False |
|   |   | year | Year of the term, e.g. 2014, 2015. | INTEGER | False | True | False |




| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Instructors  | The stuffs who teach courses in the school. The table name of the entity is "Instructors". | instructorId | Primary key of an instructor record in the table "Instructors". | INTEGER | True | True | True |
|   |   | firstName | First name of this instructor, e.g. The first name of data "Jiang, Keyuan" is "Keyuan". | TEXT | False | True | False |
|   |   | lastName | Last name of this instructor, e.g. The  last name of data  "Jiang, Keyuan" is "Jiang". | INTEGER | False | True | False |


| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Buildings  | The building in which instructors teach classes . The table name of the entity is "Buildings". | buildingId | Primary key of a building record in the table "Buildings". | INTEGER | True | True | True |
|   |   | building | Name of this building, e.g. The building name of data "Gyte (Millard E) Science Bldg - 002" is "Gyte (Millard E) Science Bldg". | TEXT | False | True | True |


| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Courses  | A series of lectures or lessons in a particular subject. The table name of the entity is "Courses". | courseId | Primary key of a course record in the table "Courses" | INTEGER | True | True | True |
|   |   | subjectId | Foreign Key of a course record indicates which subject offers this course, associated with the subject record in the table "Subjects". | TEXT | False | True | False |
|   |   | courseNum | Code of the course. The value of data cells associated with column "#" in project data. e.g. the course number of "AAE 69001"  is "69001". | TEXT | False | True | False |
|   |   | credits | Credit hours of the course. The value of data cell associated with column "Cr Hrs", e.g. "3.0". | TEXT | False | True | False |


| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Sections  | A course section is a specific instance of a course offered during a specific term | sectionId | Primary key of a section record in the table "Sections". | INTEGER | True | True | True |
|   |   | termId | Foreign Key of a section record indicates which term offered this course section, associated with the specific term record in the table "Terms". | INTEGER | False | True | False |
|   |   | courseId | Foreign Key of a section record indicates which course offered this course section, associated with the specific course record in the table "Courses". | INTEGER | False | True | False |
|   |   | CRN | In the raw data, CRN is the value of data cell associated with column "CRN", e.g. "66103", which distinguish each course section in each academic year. CRNs are recycling every academic year, which means a course section can have same CRN across years. | INTEGER | False | True | False |
|   |   | sectionNum | Code of a course section under the associated course. In the raw data, "sectionNum" is the value of data cell associated with column "Section", e.g. "01". | INTEGER | False | True | False  |
|   |   | title | Tile of a course section. In the raw data, "title" is the value of data cell associated with column "Title", e.g. "Drawing II". | TEXT | False | True | False |
|   |   | scheduleTypeId | Foreign Key of a section record indicates what type of schedule is offered with the section, associated with the specific record in table "ScheduleTypes". | INTEGER | False | True | False |
|   |   | instructorId | Foreign Key of a section record indicates which instructor teaches the section, associated with the specific record in table "Instructors". | INTEGER | False | True | False |
|   |   | meetingStart | The date in which a course section starts. In the raw data, "meetingStart" can be found in the data cell associated with column "Section Meeting Dates", e.g. "JAN 19, 2010" in "JAN 19, 2010 to MAY 15, 2010". | TEXT | False | True | False |
|   |   | meetingEnd | The date in which a course section ends. In the raw data, "meetingEnd" can be found in the data cell associated with column "Section Meeting Dates", e.g. "MAY 15, 2010" in "JAN 19, 2010 to MAY 15, 2010". | TEXT | False | True | False |
|   |   | enrlCap | The number of available seats for enrollment in a course section. "enrlCap" can be found in the value of data cells associated with column "EnrollmentTaken/Avail", e.g. "3" in "21 / 3" | INTEGER | False | True | False |
|   |   | enrlAct | The number of taken seats for enrollment in a course section. "enrlAct" can be found in the values of data cells associated with column "EnrollmentTaken/Avail", e.g. "21" in "21 / 3" | INTEGER | False | True | False |
|   |   | waitCap | The number of available seats for wait list. If the data cell associated with column "WaitlistTaken/Avail" is shown as "N/A", then "waitCap" is 0. | INTEGER | False | True | False |
|   |   | waitAct | The number of taken seats for wait list. If the data cell associated with column "WaitlistTaken/Avail" is shown as "N/A", then "waitAct" is 0. | INTEGER | False | True | False |
|   |   | isEXL | The flag that indicates if a course section is included in EXL program. If yes, "isEXL" is 1, otherwise 0. To distinguish an EXL course section, the acronym "EXL" is indicated in the values of the data cells which associated with column "#" in raw data, e.g. "14100(EXL)" | INTEGER | False | True | False |
|   |   | isETIE | The flag that indicates if a course section is included in ETIE program. If yes, "isETIE" is 1, otherwise 0. To distinguish an ETIE course section, the acronym "ETIE" is indicated in the values of the data cells which associated with column "Title" and column "Important comments about the section." in raw data, e.g. "ETIE English" in the title and "ETIE" in the comment area. | INTEGER | False | True | False |
|   |   | isTransferIN | The flag that indicates if a course section is included in TransferIN program. If yes, "isTransferIN" is 1, otherwise 0. To distinguish a TransferIN course section, the acronym "TransferIN" is indicated in the values of the data cells which associated with column "#" in raw data, e.g. "15100(TransferIN)" | INTEGER | False | True | False |
|   |   | isCanceled | The flag that indicates if a course section canceled. If yes, "isCanceled" is 1, otherwise 0. To distinguish a canceled course section, the status is indicated in the values of the data cells which associated with column "Important comments about the section" in raw data, e.g. "\*\*\*CANCELED\*\*\*" in the comment area. | INTEGER | False | True | False |
|   |   | isOnline | The flag that indicates if a course section is scheduled as distance learning. If yes, "isOnline" is 1, otherwise 0. To distinguish a distance learning course section, the key word "Distance Learning" is indicated in the values of the data cells which associated with column "Type", column "Times", column, column "Building - Room"  and column "Important comments about the section." in raw data, e.g. "Distance Learning" in the time and "Distance Learning Courses" in the column "Building - Room". | INTEGER | False | True | False |
|   |   | isSIA | The flag that indicates if a course section is supplemental instruction available. If yes, "isSIA" is 1, otherwise 0. To distinguish a SIA course section, the keyword "supplemental instruction available" is indicated in the values of the data cells which associated with column "Important comments about the section." in raw data, e.g. "Supplemental Instruction Available" in the comment area. | INTEGER | False | True | False |
|   |   | textBookLink | The hyper link linked to the textbook information of a course section. To found the link, capture the hyper link with the keyword "View Books" in comment area. | TEXT | False | True | False |
|   |   | CRNLink | The part of URL linked to the details of a course section, manipulated by JavaScript. To found the URL, capture the <a> tag in data cell associated with column "CRN" e.g. "62373". Within the <a> tag, only capture the URL that passed to JavaScript function "openWindow", e.g. "<a href="javascript:openWindow(' **/pls/proddad/Webctlg.P\_CtlgProcInput?inputsubjcode=AD&inputsymbol==&inputcrsenumb=22200&inputcoursetype=2&inputreqind=2&callpage=clistquery**')">62373</a>" | TEXT | False | True | False |
|   |   | notes | The rest information that uncovered by flags "isETIE", "isOnline", "isCanceled" and "isSIA", " **Pre-Requisites**", " **Co-Requisites**" and "View Books" in the comment area. | TEXT | False | True | False |


| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Schedule Types  | Specific name of the schedule for a course section during the weekdays. | instructorId | Primary key of scheduleType record in the table "scheduleTypes". | INTEGER | True | True | True |
|   |   | scheduleType | The type name of a course section' schedule. "scheduleType" is the value of the data cell associated with column "Type" in raw data, e.g. "Lecture" and "Distance Learning". | TEXT | False | True | False |




| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Section Times | The specific time in which the course is taught during scheduled days. The table name of the entity is "SectionTimes". | sectionTimeId | Primary key of a sectionTime record in the table "SectionTimes". | INTEGER | True | True | True |
|   |   | sectionId | Foreign key of a sectionTime record indicates which course section is scheduled at this time, associated with the specific section record  in table "Sections" | INTEGER | False | True | False |
|   |   | day | The names of scheduled days, e.g. "M", "R", "MW" and "TR" | TEXT | False | True | False |
|   |   | timeStart | The time in which a course section starts during scheduled days. In the raw data, "timeStart" can be found in the data cell associated with column "Times", e.g. "05:00 PM" in "05:00 PM - 05:50 PM". | TEXT | False | False | False |
|   |   | timeEnd | The time in which a course section ends during scheduled days. In the raw data, "timeEnd" can be found in the data cell associated with column "Times", e.g. "05:50 PM" in "05:00 PM - 05:50 PM". | TEXT | False | False | False |


| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Section Locations | The specific location at which the course is taught during scheduled days. The table name of the entity is "SectionTimes". | sectionTimeId | Primary key of a sectionLocation record in the table "SectionTimes". | INTEGER | True | True | True |
|   |   | sectionId | Foreign key of a sectionLocation record indicates which course section is scheduled at this location, associated with the specific section record in table "Sections". | INTEGER | False | True | False |
|   |   | roomNum | Code of room where the course is taught. "roonNum" can be found in the value of data cell associated with column "Building - Room" e.g. "131" in "Powers (Donald S) Building - 131" | TEXT | False | False | False |
|   |   | buildingId | Foreign key of a sectionLocation record indicates which building is scheduled for the course section, associated with the specific building record in table "Buildings". | INTEGER | False | False | False |


| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Pre-Requisites | A course that a student must pass before enrolling in the more advanced course. The table name of the entity is "PreRequisites". These data can be found in comment area with keyword "Pre-Requisites:". | prId | Primary key of an Prerequisite record in the table "PreRequisites". | INTEGER | True | True | True |
|   |   | sectionId | Foreign key of a prerequisite record indicates which course section is advanced, associated with the specific course record  in table "Courses" | INTEGER | False | True | False |
|   |   | subject | The subject name of this prerequisite, e.g. the subject name of data "Pre-Requisites: MA 15300" is "MA". | TEXT | False | True | False |
|   |   | courseNum | The course code of this prerequisite, e.g. the  course code of data  "Pre-Requisites: MA 15300" is "15300". | TEXT | False | True | False |


| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Co-Requisites | A course that a student must enroll in at the same time as, or in some cases prior to, enrolling in the desired course. The table name of the entity is "CoRequisites". These data can be found in comment area with keyword "Co-Requisites:". | prId | Primary key of a co-requisite record in the table "CoRequisites". | INTEGER | True | True | True |
|   |   | sectionId | Foreign key of a co-requisite record indicates which course section is required to enroll at same time, associated with the specific course record  in table "Courses" | INTEGER | False | True | False |
|   |   | subject | The subject name of this co-requisite, e.g. the  subject name of data  "Co-Requisites: CHM 26200" is "CHM". | TEXT | False | True | False |
|   |   | courseNum | The course code of this co-requisite, e.g. the  course code of data  "Co-Requisites: CHM 26200" is "26200". | TEXT | False | True | False |
