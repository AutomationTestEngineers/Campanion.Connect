﻿@Transfer @E2E
Feature: Transfer
	In order Execute Transfer Intake To Outcome Functionality

Background: 
	Given I Login


Scenario Outline: Transfer_Intake_To_Outcome
	When I Change Shelter "<Shelter>"	
	And I Click Add
	And I Select "Transfer" Intake
	And I Select Partner "<Person>"
	And I Add Animal
	And I Enter Behavior
	And I Enter Medical "<IntakeSection>"
	And I Enter Details
		Then User Should See Animal Name
	When I Enter Animal Details To Profile
	And I Realease Animal Holds
	And I Click New Outcome Button
	And I Select "<Outcome>"
	And I Delete Recent Outcome
	And I Delete Recent Intake
		Then "Update data successful" Message Should Be Display

	Examples: 
	| Outcome         | Shelter                         | Person | IntakeSection |
	| Death           | Demo Shelter                    | k      | 1             |
	| Return to Owner | Demo Shelter                    | k      | 3             |
	| Euthanasia      | Central Missouri Humane Society | k      | 2             |
	| Transfer        | Demo Shelter                    | k      | 4             |
	| Adoption        | Demo Shelter                    | k      | 5             |


@Intake
Scenario: Transfer_Intake
	When I Change Shelter "Demo Shelter"	
	And I Click Add
	And I Select "Transfer" Intake
	And I Select Partner "k"
	And I Add Animal
	And I Enter Behavior
	And I Enter Medical "10"
	And I Enter Details
		Then User Should See Animal Name
	When I Delete Recent Intake
		Then "Update data successful" Message Should Be Display
