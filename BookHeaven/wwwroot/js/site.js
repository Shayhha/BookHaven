// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function handleSearchButtonClick() {
    var searchQuery = document.getElementsByName("searchQuery")[0].value;
    window.location.href = '/SearchResults/showSearchResults?searchQuery=' + encodeURIComponent(searchQuery);
}

function editProfile() {
    // Toggle visibility of email input field
    var emailSpan = document.getElementById('emailSpan');
    var emailInput = document.getElementById('emailInput');
    emailInput.value = emailSpan.textContent;
    emailSpan.textContent = '';
    emailInput.style.display = (emailInput.style.display === 'none') ? 'block' : 'none';

    // Toggle visibility of first name input field
    var firstNameSpan = document.getElementById('firstNameSpan');
    var firstNameInput = document.getElementById('firstNameInput');
    firstNameInput.value = firstNameSpan.textContent;
    firstNameSpan.textContent = '';
    firstNameInput.style.display = (firstNameInput.style.display === 'none') ? 'block' : 'none';

    // Toggle visibility of last name input field
    var lastNameSpan = document.getElementById('lastNameSpan');
    var lastNameInput = document.getElementById('lastNameInput');
    lastNameInput.value = lastNameSpan.textContent;
    lastNameSpan.textContent = '';
    lastNameInput.style.display = (lastNameInput.style.display === 'none') ? 'block' : 'none';

    // Toggle visibility of address input fields
    var addressSpan = document.getElementById('addressSpan');
    var addressParts = addressSpan.textContent.split(', ');
    addressSpan.textContent = '';

    // Assign values to respective input fields
    document.getElementById('countryInput').value = addressParts.pop(); // Last part is country
    document.getElementById('cityInput').value = addressParts.pop(); // Next part is city
    document.getElementById('streetInput').value = addressParts.pop(); // Next part is street
    document.getElementById('apartmentInput').value = addressParts.pop(); // First part is apartment number

    var addressInputs = document.getElementById('address').querySelectorAll('.input-field');
    addressInputs.forEach(input => {
        input.style.display = (input.style.display === 'none') ? 'block' : 'none';
    });


    // Toggle visibility of credit card input fields
    var creditCardSpan = document.getElementById('creditCardSpan');
    var creditCardParts = creditCardSpan.textContent.split(' | ');
    creditCardSpan.textContent = '';

    // Assign values to respective input fields
    document.getElementById('ccvInput').value = creditCardParts.pop(); // Last part is country
    document.getElementById('expirationInput').value = creditCardParts.pop(); // Next part is city
    document.getElementById('cardNumberInput').value = creditCardParts.pop(); // Next part is street

    var creditCardInputs = document.getElementById('creditCard').querySelectorAll('.input-field');
    creditCardInputs.forEach(input => {
        input.style.display = (input.style.display === 'none') ? 'block' : 'none';
    });

    // Toggle visibility of delete buttons
    var deleteButtons = document.querySelectorAll('.delete-button');
    deleteButtons.forEach(button => {
        button.style.display = (button.style.display === 'none') ? 'block' : 'none';
    });

    // Change edit button to save button
    var editButton = document.querySelector('.edit-button');
    editButton.innerHTML = (editButton.innerHTML === 'Edit') ? 'Save' : 'Edit';
    editButton.setAttribute('onclick', (editButton.getAttribute('onclick') === 'editProfile()') ? 'saveProfile()' : 'editProfile()');
}

async function saveProfile() {
    var emailInput = document.getElementById('emailInput').value;
    var fnameInput = document.getElementById('firstNameInput').value;
    var lnameInput = document.getElementById('lastNameInput').value;
    var countryInput = document.getElementById('countryInput').value;
    var cityInput = document.getElementById('cityInput').value;
    var streetInput = document.getElementById('streetInput').value;
    var apartNumInput = document.getElementById('apartmentInput').value;
    var numberInput = document.getElementById('cardNumberInput').value;
    var dateInput = document.getElementById('expirationInput').value;
    var ccvInput = document.getElementById('ccvInput').value;

    var data = {
        email: emailInput,
        fname: fnameInput,
        lname: lnameInput,
        address: {
            country: countryInput,
            city: cityInput,
            street: streetInput,
            apartNum: apartNumInput
        },
        creditCard: {
            number: numberInput,
            date: dateInput,
            ccv: ccvInput
        }
    };

    // Call the saveChanges function and await its result
    const success = await saveChanges(data);

    if (success) {
        // Update email with input value
        var emailSpan = document.getElementById('emailSpan');
        emailSpan.textContent = emailInput;

        // Update first name with input value
        var firstNameSpan = document.getElementById('firstNameSpan');
        firstNameSpan.textContent = fnameInput;

        // Update last name with input value
        var lastNameSpan = document.getElementById('lastNameSpan');
        lastNameSpan.textContent = lnameInput;

        // Hide input fields
        document.getElementById('emailInput').style.display = 'none';
        document.getElementById('firstNameInput').style.display = 'none';
        document.getElementById('lastNameInput').style.display = 'none';

        // Update address with input values
        var addressSpan = document.getElementById('addressSpan');
        addressSpan.textContent = streetInput + ', ' + cityInput + ', ' + countryInput + ', ' + apartNumInput;
        var addressInputs = document.getElementById('address').querySelectorAll('.input-field');
        addressInputs.forEach(input => {
            input.style.display = 'none';
        });

        // Update credit card with input values
        var creditCardSpan = document.getElementById('creditCardSpan');
        creditCardSpan.textContent = numberInput + ' | ' + dateInput + ' | ' + ccvInput;
        var creditCardInputs = document.getElementById('creditCard').querySelectorAll('.input-field');
        creditCardInputs.forEach(input => {
            input.style.display = 'none';
        });

        // Show delete buttons
        var deleteButtons = document.querySelectorAll('.delete-button');
        deleteButtons.forEach(button => {
            button.style.display = 'inline-block';
        });

        // Change save button back to edit button
        var editButton = document.querySelector('.edit-button');
        editButton.innerHTML = 'Edit';
        editButton.setAttribute('onclick', 'editProfile()');
    }
}

async function saveChanges(data) {
    try {
        const response = await fetch('/Profile/SaveChanges', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        const responseData = await response.json();
        return responseData.success; // Return true if success is true, otherwise false
    } catch (error) {
        console.error('Error:', error);
        return false; // Return false in case of an error
    }
}


function deleteAddress() {
    // AJAX request to delete credit card information
    fetch('DeleteAddress', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({}),
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            console.log('Address deleted');
            document.getElementById('addressSpan').innerHTML = '<span></span>';
            document.querySelectorAll('.delete-button').forEach(button => button.classList.remove('hide'));
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function deleteCreditCard() {
    // AJAX request to delete credit card information
    fetch('DeleteCreditCard', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({}),
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            console.log('Credit Card deleted');
            document.getElementById('creditCardSpan').innerHTML = '<span></span>';
            document.querySelectorAll('.delete-button').forEach(button => button.classList.remove('hide'));
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}


function userLogout() {
    window.location.href = '/UserHome/userLogout';
}

function confirmDelete() {
    return confirm("Are you sure you want to delete this item?");
}

function showEmailTakenMessage() {
    alert("This email is already taken. Please choose another one.");
}