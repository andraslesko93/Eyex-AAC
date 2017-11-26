from flask import Flask
from flask import request
from flask_sqlalchemy import SQLAlchemy
from flask import render_template
from sqlalchemy.ext.associationproxy import association_proxy
from sqlalchemy.ext.orderinglist import ordering_list
import base64

import json
app = Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] = r'sqlite:///c:\Projects\Eyex-AAC\source\projects\Eyex-AAC-Server\database.db'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
db = SQLAlchemy(app)


class Messenger(db.Model, dict):
    __tablename__ = 'messenger'
    id = db.Column(db.Integer, primary_key=True)
    messenger_name = db.Column(db.String)
    messenger_usage_count = db.Column(db.Integer, default=1)
    _images = db.relationship('MessengerXImage', order_by='MessengerXImage.image_usage_count',
                              collection_class=ordering_list('image_usage_count'))
    images = association_proxy('_images', 'image', creator=lambda _i: MessengerXImage(image=_i, image_usage_count=1),)


class Image(db.Model):
    __tablename__ = 'image'
    id = db.Column(db.Integer, primary_key=True)
    encoded_messenger_image = db.Column(db.String, unique=True)


class MessengerXImage(db.Model):
    messenger_id = db.Column(db.Integer, db.ForeignKey('messenger.id'), primary_key=True)
    image_id = db.Column(db.Integer, db.ForeignKey('image.id'), primary_key=True)
    image_usage_count = db.Column(db.Integer, default=1)
    image = db.relationship('Image')


@app.route('/')
def index():
    messengers = Messenger.query.all()
    messengers.sort(key=lambda x: x.messenger_usage_count, reverse=True)
    return render_template('index.html', messengers=messengers)


@app.route('/messengers/<int:messenger_id>')
def get_messengers(messenger_id):
    messenger = Messenger.query.filter(Messenger.id == messenger_id).first()
    messenger._images.sort(key=lambda x: x.image_usage_count, reverse=True)

    usage_numbers = []
    for image in  messenger._images:
        usage_numbers.append(image.image_usage_count)
    return render_template('messenger.html', messenger=messenger, usage_numbers=usage_numbers)


@app.route('/get_top_messengers/<int:number>')
def get_top_messengers(number):
    messengers = Messenger.query.all()
    messengers.sort(key=lambda x: x.messenger_usage_count, reverse=True)
    result = messengers[:number]
    return to_json(result)


def to_json(messengers):
    json_list = []
    for messenger in messengers:
        json_list.append({
            'MessengerName': messenger.messenger_name,
            'MessengerUsageCount':  messenger.messenger_usage_count})
    return json.dumps(json_list, ensure_ascii=False).encode('utf8')


@app.route('/log', methods=['POST'])
def log():
    for list_item in request.get_json():
        normalized_messenger_name = normalize(list_item.get("MessengerName", None))
        encoded_messenger_image = list_item.get("EncodedMessengerImage", None)
        image = Image(encoded_messenger_image=encoded_messenger_image)
        existing_image = Image.query.filter(Image.encoded_messenger_image == encoded_messenger_image).first()
        existing_messenger = Messenger.query.filter(Messenger.messenger_name == normalized_messenger_name).first()
        if existing_messenger is None:
            messenger = Messenger(messenger_name=normalized_messenger_name)
            if existing_image is None:
                messenger.images.append(image)
            else:
                messenger.images.append(existing_image)
            db.session.add(messenger)
        else:
            if existing_image is None:
                existing_messenger.images.append(image)
            else:
                messenger_image_relation = MessengerXImage.query\
                    .filter(MessengerXImage.messenger_id == existing_messenger.id)\
                    .filter(MessengerXImage.image_id == existing_image.id).first()
                messenger_image_relation.image_usage_count += 1
                db.session.add(messenger_image_relation)
            existing_messenger.messenger_usage_count += 1
            db.session.add(existing_messenger)
    db.session.commit()
    return "200"


def normalize(messenger_name):
    return messenger_name.lower().replace(" ", "")


if __name__ == "__main__":
    app.run(debug=True)
