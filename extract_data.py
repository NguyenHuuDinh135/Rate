import json
import re

def extract_movies(content):
    # This regex is simplified and might need adjustment depending on the exact content
    movie_pattern = re.compile(r'new Movie\s*\{(.*?)\}', re.DOTALL)
    movies = []
    for match in movie_pattern.finditer(content):
        movie_text = match.group(1)
        movie = {}
        title = re.search(r'Title\s*=\s*"(.*?)"', movie_text)
        summary = re.search(r'Summary\s*=\s*"(.*?)"', movie_text, re.DOTALL)
        year = re.search(r'Year\s*=\s*(\d+)', movie_text)
        rating = re.search(r'Rating\s*=\s*([\d\.]+)m', movie_text)
        trailer = re.search(r'TrailerUrl\s*=\s*"(.*?)"', movie_text)
        poster = re.search(r'PosterUrl\s*=\s*"(.*?)"', movie_text)
        mtype = re.search(r'MovieType\s*=\s*MovieType\.(ComingSoon|NowShowing|Removed)', movie_text)
        
        if title: movie['Title'] = title.group(1)
        if summary: movie['Summary'] = summary.group(1).replace('\n', ' ').replace('  ', ' ')
        if year: movie['Year'] = int(year.group(1))
        if rating: movie['Rating'] = float(rating.group(1))
        else: movie['Rating'] = None
        if trailer: movie['TrailerUrl'] = trailer.group(1)
        if poster: movie['PosterUrl'] = poster.group(1)
        if mtype: movie['MovieType'] = mtype.group(1)
        
        movies.append(movie)
    return movies

def extract_movie_genres(content):
    pattern = re.compile(r'new MovieGenre\s*\{\s*MovieId\s*=\s*(\d+),\s*GenreId\s*=\s*(\d+)\s*\}')
    items = []
    for match in pattern.finditer(content):
        items.append({
            "MovieId": int(match.group(1)),
            "GenreId": int(match.group(2))
        })
    return items

def extract_persons(content):
    pattern = re.compile(r'new Person\s*\{\s*FullName\s*=\s*"(.*?)"\s*,\s*Age\s*=\s*(\d+)\s*,\s*PictureUrl\s*=\s*"(.*?)"\s*\}')
    items = []
    for match in pattern.finditer(content):
        items.append({
            "FullName": match.group(1),
            "Age": int(match.group(2)),
            "PictureUrl": match.group(3)
        })
    return items

def extract_movie_persons(content):
    pattern = re.compile(r'new MoviePerson\s*\{\s*MovieId\s*=\s*(\d+),\s*PersonId\s*=\s*(\d+),\s*RoleType\s*=\s*RoleType\.(Cast|Director|Producer)\s*\}')
    items = []
    for match in pattern.finditer(content):
        items.append({
            "MovieId": int(match.group(1)),
            "PersonId": int(match.group(2)),
            "RoleType": match.group(3)
        })
    return items

def extract_theaters(content):
    pattern = re.compile(r'new Theater\s*\{\s*Name\s*=\s*"(.*?)"\s*,\s*NumOfRows\s*=\s*(\d+)\s*,\s*SeatsPerRow\s*=\s*(\d+)\s*,\s*Type\s*=\s*TheaterType\.(Normal|Royal)\s*\}')
    items = []
    for match in pattern.finditer(content):
        items.append({
            "Name": match.group(1),
            "NumOfRows": int(match.group(2)),
            "SeatsPerRow": int(match.group(3)),
            "Type": match.group(4)
        })
    return items

def extract_theater_seats(content):
    pattern = re.compile(r'new TheaterSeat\s*\{\s*SeatRow\s*=\s*"(.*?)"\s*,\s*SeatNumber\s*=\s*(\d+)\s*,\s*TheaterId\s*=\s*(\d+)\s*,\s*Type\s*=\s*SeatType\.(Missing|Occupied|Reserved|Normal)\s*\}')
    items = []
    for match in pattern.finditer(content):
        items.append({
            "SeatRow": match.group(1),
            "SeatNumber": int(match.group(2)),
            "TheaterId": int(match.group(3)),
            "Type": match.group(4)
        })
    return items

def extract_bookings(content):
    # Use re.DOTALL to handle multiline if necessary, but these are usually single line
    pattern = re.compile(r'new Booking\s*\{\s*UserId\s*=\s*(user\d+Id),\s*ShowId\s*=\s*(\d+),\s*SeatRow\s*=\s*"(.*?)",\s*SeatNumber\s*=\s*(\d+),\s*Price\s*=\s*(\d+),\s*Status\s*=\s*BookingStatus\.(Confirmed|Reserved),\s*BookingDateTime\s*=\s*DateTime\.Parse\("(.*?)"\)\s*\}')
    items = []
    for match in pattern.finditer(content):
        items.append({
            "UserKey": match.group(1),
            "ShowId": int(match.group(2)),
            "SeatRow": match.group(3),
            "SeatNumber": int(match.group(4)),
            "Price": float(match.group(5)),
            "Status": match.group(6),
            "BookingDateTime": match.group(7)
        })
    return items

def extract_payments(content):
    pattern = re.compile(r'new Payment\s*\{\s*Amount\s*=\s*(\d+),\s*PaymentDateTime\s*=\s*DateTime\.Parse\("(.*?)"\),\s*Method\s*=\s*PaymentMethod\.(Card|Cash|Wallet),\s*UserId\s*=\s*(user\d+Id),\s*ShowId\s*=\s*(\d+)\s*\}')
    items = []
    for match in pattern.finditer(content):
        items.append({
            "Amount": float(match.group(1)),
            "PaymentDateTime": match.group(2),
            "Method": match.group(3),
            "UserKey": match.group(4),
            "ShowId": int(match.group(5))
        })
    return items

def extract_users(content):
    # This matches the user definitions like: var user1Id = "...";
    user_id_pattern = re.compile(r'var (user\d+Id)\s*=\s*"(.*?)";')
    user_ids = {}
    for match in user_id_pattern.finditer(content):
        user_ids[match.group(1)] = match.group(2)
        
    # This matches the users array
    users_pattern = re.compile(r'new\s*\{\s*Id\s*=\s*(user\d+Id),\s*Email\s*=\s*"(.*?)",\s*Password\s*=\s*"(.*?)",\s*Address\s*=\s*"(.*?)",\s*Contact\s*=\s*"(.*?)",\s*Role\s*=\s*"(.*?)"\s*\}')
    users = []
    for match in users_pattern.finditer(content):
        key = match.group(1)
        users.append({
            "Id": user_ids.get(key, key),
            "UserKey": key,
            "Email": match.group(2),
            "Password": match.group(3),
            "Address": match.group(4),
            "Contact": match.group(5),
            "Role": match.group(6)
        })
    return users

with open('backend/src/Infrastructure/Data/ApplicationDbContextInitialiser.cs', 'r') as f:
    content = f.read()

data_dir = 'backend/src/Infrastructure/Data/SeedData/'

def save_json(name, data):
    with open(f'{data_dir}{name}.json', 'w') as f:
        json.dump(data, f, indent=2)

save_json('Movies', extract_movies(content))
save_json('MovieGenres', extract_movie_genres(content))
save_json('Persons', extract_persons(content))
save_json('MoviePersons', extract_movie_persons(content))
save_json('Theaters', extract_theaters(content))
save_json('TheaterSeats', extract_theater_seats(content))
save_json('Bookings', extract_bookings(content))
save_json('Payments', extract_payments(content))
save_json('Users', extract_users(content))
