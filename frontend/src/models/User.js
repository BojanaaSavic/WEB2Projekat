export class User {
  constructor(id, name, email, role = 'User') {
    this.id = id;
    this.name = name;
    this.email = email;
    this.role = role;
  }
}

export class LoginRequest {
  constructor(email, password) {
    this.email = email;
    this.password = password;
  }
}

export class RegisterRequest {
  constructor(name, email, password) {
    this.name = name;
    this.email = email;
    this.password = password;
  }
}

export class AuthResponse {
  constructor(token, user) {
    this.token = token;
    this.user = user;
  }
}
