from flask import Flask
from flask import request
from flask_sqlalchemy import SQLAlchemy
import json
app = Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] = r'sqlite:///c:\Projects\Eyex-AAC\source\projects\Eyex-AAC-Server\database.db'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
db = SQLAlchemy(app)


class Messenger(db.Model, dict):
    id = db.Column(db.Integer, primary_key=True)
    messenger_name = db.Column(db.String)
    encoded_messenger_image = db.Column(db.String)


@app.route('/')
def index():
    messengers = Messenger.query.all()
    return to_json(messengers)


def to_json(messengers):
    json_list = []
    for messenger in messengers:
        json_list.append({
            'MessengerName': messenger.messenger_name,
            'EncodedMessengerImage':  messenger.encoded_messenger_image})
    return json.dumps(json_list, ensure_ascii=False).encode('utf8')


@app.route('/log', methods=['POST'])
def log():
    print(request.get_json())

    for list_item in request.get_json():
        messenger_name = list_item.get("MessengerName", None)
        encoded_messenger_image = list_item.get("EncodedMessengerImage", None)
        messenger = Messenger(messenger_name=messenger_name, encoded_messenger_image=encoded_messenger_image)
        db.session.add(messenger)
    db.session.commit()
    return "200"


if __name__ == "__main__":
    app.run(debug=True)

