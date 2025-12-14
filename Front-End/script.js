// Base URL for the ASP.NET backend API
let url = "https://localhost:7158/";

// Run when the DOM is fully loaded
// Calls the StartUp endpoint to load initial data
$(document).ready(StartUp);

function StartUp() {
    CallAJAX(url + "StartUp", 'get', {}, 'json', StartSuccess, Err);
}
// Global variable to store returned data for reuse
let data;

/**
 * Handles initial data load from the server.
 * Builds the students table dynamically and attaches event handlers.
*/
function StartSuccess(returnedData) {
    data = returnedData;

    // Extract students array from returned JSON
    let students = returnedData['students'];

    // First row contains column titles
    let titles = students[0];

    // Build the students table dynamically
    $("#table").html(function () {
        let output = "";

        // Table header row
        output += `<tr>
            <th>Get Students</th>
            <th>${titles[0]}</th>
            <th>${titles[1]}</th>
            <th>${titles[2]}</th>
            <th>${titles[3]}</th>
            <th>Action</th>
        </tr>`;

        // Populate table rows
        for (let i = 1; i < students.length; i++) {
            output += `<tr style="width: 100%;" id="row${students[i][0]}">
                <td><button id="${i}" class="btn">Retrieve Class Info</button></td>
                <td>${students[i][0]}</td>
                <td>${students[i][1]}</td>
                <td>${students[i][2]}</td>
                <td>${students[i][3]}</td>
                <td>
                    <button type="button" class="delete" id="${students[i][0]}">Delete</button>
                    <button type="button" class="edit" id="${students[i][0]}">Edit</button>
                </td>
            </tr>`;
        }

        return output;
    });

    // Display record count
    $("#tableStatus").html(`<b> Retrieved ${students.length - 1} Records</b>`);

    // Attach event handlers
    $(".btn").on("click", RetrieveData);
    $(".delete").click(DeleteFunc);
    $(".edit").click(EditFunc);
}

// Retrieves class information for a selected student.

function RetrieveData() {
    let tabledata = data['students'];

    // Use button ID as index to locate student ID
    let id = tabledata[this.id][0];

    CallAJAX(url + `Retrieve?id=${id}`, 'GET', {}, 'json', RetrieveSuccess, Err);
}

/*
 * Handles successful retrieval of class data.
 * Builds the classes table dynamically.
 */
function RetrieveSuccess(returnedData) {
    let classes = returnedData["table"];

    $("#data").html(function () {
        let output = "";

        // Table header
        output += `<tr>
            <th>Class ID</th>
            <th>Class Desc</th>
            <th>Days</th>
            <th>Start Date</th>
            <th>Instructor ID</th>
            <th>Instructor First Name</th>
            <th>Instructor Last Name</th>
        </tr>`;

        // Populate table rows
        for (let i = 1; i < classes.length; i++) {
            output += `<tr>
                <td>${classes[i][0]}</td>
                <td>${classes[i][1]}</td>
                <td>${classes[i][2]}</td>
                <td>${classes[i][3]}</td>
                <td>${classes[i][4]}</td>
                <td>${classes[i][5]}</td>
                <td>${classes[i][6]}</td>
            </tr>`;
        }

        return output;
    });

    $("#dataStatus").html(`<b> Retrieved ${classes.length - 1} Records</b>`);
}

//Sends a delete request for the selected student.

function DeleteFunc() {
    CallAJAX(url + `Delete?id=${this.id}`, 'delete', {}, 'json', DeleteSuccess, Err);
}

/**
 * Handles successful delete operation and refreshes table.
 */
function DeleteSuccess(returnedData) {
    let status = returnedData["rows"];
    console.log(`message:${returnedData["message"]} status:${returnedData["status"]}`);
    $("#dataStatus").html(`Deleted ${status} rows`);

    // Reload updated student data
    StartUp(returnedData);
}

/**
 * Enables inline editing for a selected student row.
 */
function EditFunc() {
    let rowid = this.id;
    let students = data['students'];
    let student;

    // Locate selected student
    for (let i = 0; i < students.length; i++) {
        if (students[i][0] == rowid) {
            student = students[i];
        }
    }

    // Replace row with editable inputs
    $(`#row${rowid}`).html(function () {
        let output = "";

        output += `<td><button>Retrieve Class Info</button></td>`;
        output += `<td>${student[0]}</td>`;
        output += `<td><input type="text" value="${student[1]}" id="FName${student[0]}"></td>`;
        output += `<td><input type="text" value="${student[2]}" id="LName${student[0]}"></td>`;
        output += `<td><input type="text" value="${student[3]}" id="SchoolID${student[0]}"></td>`;
        output += `<td>
            <button type="button" class="update" id="${student[0]}">Update</button>
            <button type="button" class="cancel" id="${student[0]}">Cancel</button>
        </td>`;

        return output;
    });
    $(".update").click(updateFunc);
    $(".cancel").click(StartUp);
}

//send user input to server to update information
function updateFunc() {
    let id = this.id;

    let putData = {};
    putData['id'] = id;
    putData['FName'] = $(`#FName${id}`).val();
    putData['LName'] = $(`#LName${id}`).val();
    putData['SchoolID'] = $(`#SchoolID${id}`).val();

    CallAJAX(url + 'Update', 'put', putData, 'json', UpdateSuccess, Err);

}

//adter updating, retrieve new information 
function UpdateSuccess(returnedData) {
    let status = returnedData["rows"];
    $("#dataStatus").html(`updated ${status} rows`);
    StartUp();
}

/**
 * Generic AJAX helper function for API calls.
 */
function CallAJAX(url, method, data, dataType, success, error) {
    let ajaxOptions = {
        url: url,
        method: method,
        data: JSON.stringify(data),
        dataType: dataType,
        contentType: "application/json"
    };
    console.log(ajaxOptions);
    let request = $.ajax(ajaxOptions);

    request.done(success);
    request.fail(error);
}

/**
 * Generic AJAX error handler.
 */
function Err(response, status) {
    console.log(response);
    console.log(status);
}
