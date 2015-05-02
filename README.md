# PUC-ScheduleMaker

## Introduction

This project involves redesign of the PUC online class schedule application. The deliverable will be a desktop application for PUC class schedules, with easy to use and more functional features. In this report, the redesigned database for storing course information will be introduced in the following sections.

## Database Logical Design
  Based on the study of data provided by instructor, the unified schema (ERD) for class schedule is created, shown as the following: 
  ![alt text](https://github.com/EdwardTang/PUC-ScheduleMaker/blob/master/Data%20Model/ITS462_data_model.jpg)
## Data Dictionary

| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Subject  | A branch knowledge taught in a school. The table name of the entity is "Sujects". | subjectId | Primary key of a subject record in the table "Sujects" | INTEGER | True | True   | True |
| subject | Subject  name in acronym, e.g. CGT, ITS. | TEXT | False | True | True |
|   |   |   |   |   |   |   |   |

| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Terms  | The time during which school holds classes. The table name of the entity is "Terms". | termId | Primary key of a term record in the table "Terms" | INTEGER | True | True | True |
| quarter | Quarter name in each term, e.g. Fall, Spring ,Summer. | TEXT | False | True | False |
| year | Year of the term, e.g. 2014, 2015 | INTEGER | False | True | False |
|   |   |   |   |   |   |   |   |



| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Instructors  | The stuffs who teach courses  in the school. The table name of the entity is "Instructors". | instructorId | Primary key of a instructor record in the table "Instructors" | INTEGER | True | True | True |
| firstName | First nameof this insructor, e.g. The  first name of data "Jiang, Keyuan" is "Keyuan" | TEXT | False | True | False |
| lastName | Last nameof this insructor, e.g. The  last name of data  "Jiang, Keyuan" is "Jiang" | INTEGER | False | True | False |
|   |   |   |   |   |   |   |   |

| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Buildings  | The building in which instructors teach classes . The table name of the entity is "Buildings". | buildingId | Primary key of a building record in the table "Buildings" | INTEGER | True | True | True |
| building | Nameof this building, e.g. The  building name of  data"Gyte (Millard E) Science Bldg - 002" is "Gyte (Millard E) Science Bldg" | TEXT | False | True | True |
|   |   |   |   |   |   |   |   |

| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Courses  | A series of lectures or lessons in a particular subject. The table name of the entity is "Courses". | courseId | Primary key of a course record in the table "Courses" | INTEGER | True | True | True |
| subjectId | Forein Key of a course record indicates which subject offers this course,associated with the subject record in the table "Subjects" | TEXT | False | True | False |
| lastName | Last nameof this insructor, e.g. The  last name of "Jiang, Keyuan" is "Jiang" | INTEGER | False | True | False |
|   |   |   |   |   |   |   |   |
|   |   |   |   |   |   |   |   |
|   |   |   |   |   |   |   |   |

| Entity Name | Entity Description | Column Name | Column Description | Data Type | Primary Key | Not Null | Unique |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Buildings  | The building in which instructors teach classes . The table name of the entity is "Buildings". | buildingId | Primary key of instructor record in the table "Instructors" | INTEGER | True | True | True |
| firstName | First nameof this insructor, e.g. The  first name of "Jiang, Keyuan" is "Keyuan" | TEXT | False | True | False |
| lastName | Last nameof this insructor, e.g. The  last name of "Jiang, Keyuan" is "Jiang" | INTEGER | False | True | False |
|   |   |   |   |   |   |   |   |
|   |   |   |   |   |   |   |   |
|   |   |   |   |   |   |   |   |
