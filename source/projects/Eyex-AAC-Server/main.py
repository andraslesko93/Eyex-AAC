from flask import Flask
from flask import request
from flask_sqlalchemy import SQLAlchemy
from flask import render_template

import json
app = Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] = r'sqlite:///c:\Projects\Eyex-AAC\source\projects\Eyex-AAC-Server\database.db'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
db = SQLAlchemy(app)

messenger_x_image = db.Table('messenger_x_image', db.Model.metadata,
                             db.Column('messenger_id', db.Integer, db.ForeignKey('messenger.id')),
                             db.Column('image_id', db.Integer, db.ForeignKey('image.id'))
                             )


class Messenger(db.Model, dict):
    __tablename__ = 'messenger'
    id = db.Column(db.Integer, primary_key=True)
    messenger_name = db.Column(db.String)
    images = db.relationship("Image", secondary=messenger_x_image)


class Image(db.Model):
    __tablename__ = 'image'
    id = db.Column(db.Integer, primary_key=True)
    messengers = db.relationship("Messenger", secondary=messenger_x_image)
    encoded_messenger_image = db.Column(db.String, unique=True)


@app.route('/')
def index():
    messengers = Messenger.query.group_by(Messenger.messenger_name).all()
    return render_template('index.html', messengers=messengers)


@app.route('/get_messengers')
def get_messengers():
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
        normalized_messenger_name = normalize(list_item.get("MessengerName", None))
        encoded_messenger_image = list_item.get("EncodedMessengerImage", None)
        #Query by image...
        messenger = Messenger(messenger_name=normalized_messenger_name)
        existing_image = Image.query.filter(Image.encoded_messenger_image == encoded_messenger_image).first()
        if existing_image is None:
            image = Image(encoded_messenger_image=encoded_messenger_image)
            image.messengers.append(messenger)
            db.session.add(image)
        else:
            existing_image.messengers.append(messenger)
            db.session.add(existing_image)
    db.session.commit()
    return "200"


def normalize(messenger_name):
    return messenger_name.lower().replace(" ", "")


if __name__ == "__main__":
    app.run(debug=True)
