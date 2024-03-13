﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function handleSearchButtonClick() {
    var searchQuery = document.getElementsByName("searchQuery")[0].value;
    window.location.href = '/SearchResults/showSearchResults?searchQuery=' + encodeURIComponent(searchQuery);
}

function handleCategoryButtonClick(element) {
    var searchQuery = element.textContent.trim();
    window.location.href = '/SearchResults/showCategoryResults?searchQuery=' + encodeURIComponent(searchQuery);
}


function showPopup(url, name, author, date, bookId, category, format, ageLimitation, price, stock, salePrice) {
    document.getElementById('popup-image').setAttribute('src', url);
    document.getElementById('popup-title').innerHTML = name;
    document.getElementById('popup-bookId').innerHTML = bookId;
    document.getElementById('popup-author').innerHTML = author;
    document.getElementById('popup-date').innerHTML = date;
    document.getElementById('popup-category').innerHTML = category;
    document.getElementById('popup-format').innerHTML = format;
    document.getElementById('popup-age').innerHTML = ageLimitation; 
    document.getElementById('popup-stock').innerHTML = stock;

    if (salePrice != 0 && price != salePrice)
        document.getElementById('popup-price').innerHTML = '<strong>$' + salePrice + '</strong> <strike>$' + price + '</strike>';
    else
        document.getElementById('popup-price').innerHTML = '$' + price;


    document.getElementById('overlay').style.display = 'block';
    document.getElementById('popup').style.display = 'block';
}

function closePopup() {
    document.getElementById('overlay').style.display = 'none';
    document.getElementById('popup').style.display = 'none';
}


function editBook() {
    var bookId = document.getElementById("popup-bookId").textContent;
    var editUrl = "/SearchResults/showBookDetails?bookId=" + bookId;
    window.location.href = editUrl;
}

function buyBook() {
    var bookId = document.getElementById("popup-bookId").textContent;
    var editUrl = "/Payment/showPaymentView?bookId=" + bookId;
    window.location.href = editUrl;
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
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function confirmDelete() {
    return confirm("Are you sure you want to delete this item?");
}

function deleteBook() {
    var bookId = document.getElementById("popup-bookId").textContent;
    var editUrl = "/Book/deleteBook?bookId=" + bookId;

    closePopup();

    window.location.href = editUrl;
}


function addBookToCart() {
    var bookId = document.getElementById("popup-bookId").textContent;
    console.log(bookId)
    // Make an AJAX request to call the addBookToCart method
    fetch(`/Cart/addBookToCart?bookId=${bookId}`)
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert('Book added to cart successfully!');
                // Optionally, you can update the cart display on the page
                // Here, you can reload the page or update the cart information dynamically
                // For example, you could update the total price, the number of items in the cart, etc.
                // window.location.reload(); // Reload the page
                // updateCartInformation(); // Update the cart information dynamically
            } else {
                alert('Failed to add book to cart. Please try again later.');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while adding the book to cart. Please try again later.');
        });
}



function editUserInfoButtonClick() {
    window.location.href = '/Profile/showEditProfileView';
}

function userLogout() {
    window.location.href = '/UserHome/userLogout';
}

function showEmailTakenMessage() {
    alert("This email is already taken. Please choose another one.");
}
